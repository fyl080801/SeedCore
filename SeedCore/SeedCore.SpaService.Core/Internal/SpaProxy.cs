using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace SeedCore.SpaService.Internal
{
    internal static class SpaProxy
    {
        private const int DefaultWebSocketBufferSize = 4096;
        private const int StreamCopyBufferSize = 81920;
        private static readonly string[] NotForwardedHttpHeaders = new[] { "Connection" };

        private static readonly string[] NotForwardedWebSocketHeaders = new[] { "Accept", "Connection", "Host", "User-Agent", "Upgrade", "Sec-WebSocket-Key", "Sec-WebSocket-Version" };

        public static HttpClient CreateHttpClientForProxy(TimeSpan requestTimeout)
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseCookies = false,
            };

            return new HttpClient(handler)
            {
                Timeout = requestTimeout
            };
        }

        public static async Task<bool> PerformProxyRequest(
            HttpContext context,
            HttpClient httpClient,
            Task<Uri> baseUriTask,
            CancellationToken applicationStoppingToken,
            bool proxy404s)
        {
            var proxyCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(
                context.RequestAborted,
                applicationStoppingToken).Token;

            var baseUri = await baseUriTask;
            var targetUri = new Uri(
                baseUri,
                context.Request.Path + context.Request.QueryString);

            try
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    await AcceptProxyWebSocketRequest(context, ToWebSocketScheme(targetUri), proxyCancellationToken);
                    return true;
                }
                else
                {
                    using (var requestMessage = CreateProxyHttpRequest(context, targetUri))
                    {
                        using (var responseMessage = await httpClient.SendAsync(
                            requestMessage,
                            HttpCompletionOption.ResponseHeadersRead,
                            proxyCancellationToken))
                        {
                            if (!proxy404s)
                            {
                                if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                                {
                                    return false;
                                }
                            }

                            await CopyProxyHttpResponse(context, responseMessage, proxyCancellationToken);
                            return true;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                return true;
            }
            catch (IOException)
            {
                return true;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(
                    $"Failed to proxy the request to {targetUri.ToString()}, because the request to " +
                    $"the proxy target failed. Check that the proxy target server is running and " +
                    $"accepting requests to {baseUri.ToString()}.\n\n" +
                    $"The underlying exception message was '{ex.Message}'." +
                    $"Check the InnerException for more details.", ex);
            }
        }

        private static HttpRequestMessage CreateProxyHttpRequest(HttpContext context, Uri uri)
        {
            var request = context.Request;

            var requestMessage = new HttpRequestMessage();
            var requestMethod = request.Method;
            if (!HttpMethods.IsGet(requestMethod) &&
                !HttpMethods.IsHead(requestMethod) &&
                !HttpMethods.IsDelete(requestMethod) &&
                !HttpMethods.IsTrace(requestMethod))
            {
                var streamContent = new StreamContent(request.Body);
                requestMessage.Content = streamContent;
            }

            foreach (var header in request.Headers)
            {
                if (NotForwardedHttpHeaders.Contains(header.Key, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) && requestMessage.Content != null)
                {
                    requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            requestMessage.Headers.Host = uri.Authority;
            requestMessage.RequestUri = uri;
            requestMessage.Method = new HttpMethod(request.Method);

            return requestMessage;
        }

        private static async Task CopyProxyHttpResponse(HttpContext context, HttpResponseMessage responseMessage, CancellationToken cancellationToken)
        {
            context.Response.StatusCode = (int)responseMessage.StatusCode;
            foreach (var header in responseMessage.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            context.Response.Headers.Remove("transfer-encoding");

            using (var responseStream = await responseMessage.Content.ReadAsStreamAsync())
            {
                await responseStream.CopyToAsync(context.Response.Body, StreamCopyBufferSize, cancellationToken);
            }
        }

        private static Uri ToWebSocketScheme(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            var uriBuilder = new UriBuilder(uri);
            if (string.Equals(uriBuilder.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                uriBuilder.Scheme = "wss";
            }
            else if (string.Equals(uriBuilder.Scheme, "http", StringComparison.OrdinalIgnoreCase))
            {
                uriBuilder.Scheme = "ws";
            }

            return uriBuilder.Uri;
        }

        private static async Task<bool> AcceptProxyWebSocketRequest(HttpContext context, Uri destinationUri, CancellationToken cancellationToken)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (destinationUri == null)
            {
                throw new ArgumentNullException(nameof(destinationUri));
            }

            using (var client = new ClientWebSocket())
            {
                foreach (var headerEntry in context.Request.Headers)
                {
                    if (!NotForwardedWebSocketHeaders.Contains(headerEntry.Key, StringComparer.OrdinalIgnoreCase))
                    {
                        try
                        {
                            client.Options.SetRequestHeader(headerEntry.Key, headerEntry.Value);
                        }
                        catch (ArgumentException)
                        {

                        }
                    }
                }

                try
                {
                    await client.ConnectAsync(destinationUri, cancellationToken);
                }
                catch (WebSocketException)
                {
                    context.Response.StatusCode = 400;
                    return false;
                }

                using (var server = await context.WebSockets.AcceptWebSocketAsync(client.SubProtocol))
                {
                    var bufferSize = DefaultWebSocketBufferSize;
                    await Task.WhenAll(
                        PumpWebSocket(client, server, bufferSize, cancellationToken),
                        PumpWebSocket(server, client, bufferSize, cancellationToken));
                }

                return true;
            }
        }

        private static async Task PumpWebSocket(WebSocket source, WebSocket destination, int bufferSize, CancellationToken cancellationToken)
        {
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            }

            var buffer = new byte[bufferSize];

            while (true)
            {
                var resultTask = source.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    if (resultTask.IsCompleted)
                    {
                        break;
                    }

                    await Task.Delay(100);
                }

                var result = resultTask.Result;
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    if (destination.State == WebSocketState.Open || destination.State == WebSocketState.CloseReceived)
                    {
                        await destination.CloseOutputAsync(source.CloseStatus.Value, source.CloseStatusDescription, cancellationToken);
                    }

                    return;
                }

                await destination.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, cancellationToken);
            }
        }
    }
}
