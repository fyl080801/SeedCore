using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using SeedCore.SpaService.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell.Builders;

namespace Microsoft.AspNetCore.SpaServices
{
    public static class SpaBuilderExtensions
    {
        public static void UseProxyToSpaDevelopmentServer(
            this ISpaBuilder spaBuilder,
            string baseUri)
        {
            UseProxyToSpaDevelopmentServer(
                spaBuilder,
                new Uri(baseUri));
        }

        public static void UseProxyToSpaDevelopmentServer(
            this ISpaBuilder spaBuilder,
            Uri baseUri)
        {
            UseProxyToSpaDevelopmentServer(
                spaBuilder,
                () => Task.FromResult(baseUri));
        }

        public static void UseProxyToSpaDevelopmentServer(
                    this ISpaBuilder spaBuilder,
                    Func<Task<Uri>> baseUriTaskFactory)
        {
            var applicationBuilder = spaBuilder.ApplicationBuilder;
            var applicationStoppingToken = GetStoppingToken(applicationBuilder);
            var shellContext = spaBuilder.ApplicationBuilder.ApplicationServices.GetService<ShellContext>();

            applicationBuilder.UseWebSockets();

            var neverTimeOutHttpClient =
                SpaProxy.CreateHttpClientForProxy(Timeout.InfiniteTimeSpan);

            applicationBuilder.Use(async (context, next) =>
            {
                // bool isSpaRequest = ComparePath(shellContext.Settings.RequestUrlPrefix, context.Request.Path.Value, "");

                // if (isSpaRequest)
                // {
                //     await SpaProxy.PerformProxyRequest(
                //         context, neverTimeOutHttpClient, baseUriTaskFactory(), applicationStoppingToken,
                //         proxy404s: true);
                // }
                var path = context.Request.Path.Value;
                var referer = context.Request.Headers["Referer"];
                var route = context.Request.RouteValues;

                // route 是否是默认路由
                // path 是否是默认区域
                // referer 是否是默认区域

                await SpaProxy.PerformProxyRequest(
                    context,
                    neverTimeOutHttpClient,
                    baseUriTaskFactory(),
                    applicationStoppingToken,
                    proxy404s: true);
            });
        }

        private static CancellationToken GetStoppingToken(IApplicationBuilder appBuilder)
        {
            var applicationLifetime = appBuilder
                .ApplicationServices
                .GetService(typeof(IHostApplicationLifetime));
            return ((IHostApplicationLifetime)applicationLifetime).ApplicationStopping;
        }

        // private static bool ComparePath(string prefix, string requestPath, string spaPath)
        // {
        //     var pathArray = requestPath.Split("/");

        //     if (pathArray.Length > 0)
        //     {
        //         if (prefix == pathArray[0])
        //         {
        //             // isSpaRequest = ComparePath(string.Join("/", pathArray.Skip(1).ToArray()), "");
        //         }
        //         else
        //         {
        //             // isSpaRequest = ComparePath()
        //         }
        //     }

        //     return false;
        // }
    }
}
