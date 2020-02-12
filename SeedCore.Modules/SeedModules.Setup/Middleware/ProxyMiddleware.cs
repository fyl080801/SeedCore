using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SeedModules.Setup.Middleware
{
    public class ProxyMiddleware
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly RequestDelegate _nextRequestDelegate;
        private static readonly Uri _targetUri = new Uri("http://localhost:8080");

        public ProxyMiddleware(RequestDelegate nextMiddleware)
        {
            _nextRequestDelegate = nextMiddleware;
        }

        public async Task Invoke(HttpContext context)
        {
            bool validateUri = false;
            if (context.Request.Method == HttpMethods.Get)
            {
                validateUri = true;
            }
            if (validateUri == true)
            {
                var targetRequestMessage = CreateTargetMessage(context);
                using (var responseMessage = await _httpClient.SendAsync(targetRequestMessage))
                {
                    context.Response.StatusCode = (int)responseMessage.StatusCode;
                    CloneResponseHeadersIntoContext(context, responseMessage);
                    await responseMessage.Content.CopyToAsync(context.Response.Body);
                }
                return;
            }
            await _nextRequestDelegate(context);
        }

        private void CloneRequestContentAndHeaders(HttpContext context, HttpRequestMessage requestMessage)
        {
            foreach (var header in context.Request.Headers)
            {
                requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }

        private HttpRequestMessage CreateTargetMessage(HttpContext context)
        {
            var requestMessage = new HttpRequestMessage();
            CloneRequestContentAndHeaders(context, requestMessage);
            requestMessage.RequestUri = new Uri(_targetUri.AbsoluteUri + context.Request.Path);
            requestMessage.Headers.Host = _targetUri.Host;
            requestMessage.Method = new HttpMethod(context.Request.Method);
            return requestMessage;
        }

        private void CloneResponseHeadersIntoContext(HttpContext context, HttpResponseMessage responseMessage)
        {
            foreach (var header in responseMessage.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }
            foreach (var header in responseMessage.Content.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }
            context.Response.Headers.Remove("Transfer-Encoding");
        }
    }

}