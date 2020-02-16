using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using OrchardCore.Environment.Shell.Builders;
using SeedCore.SpaService;
using SeedCore.SpaService.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SeedSpaBuilderExtensions
    {
        public static void UseSpaDevelopmentServer(this ISeedSpaBuilder spaBuilder, string npmScript)
        {
            if (spaBuilder == null)
            {
                throw new ArgumentNullException(nameof(spaBuilder));
            }

            // object script = spaBuilder.Options;
            var attribute = Assembly.GetCallingAssembly().GetCustomAttribute<SpaProjectAttribute>();

            SpaServiceMiddleware.Attach(spaBuilder, Path.Combine(attribute.Project, attribute.Path), npmScript);
        }

        // public static void MapAreaControllerRoute(this ISeedSpaBuilder spaBuilder, string name, string areaName, string pattern, object defaults = null, object constraints = null, object dataTokens = null)
        // {

        //     spaBuilder.ApplicationBuilder.ApplicationServices.GetService<IEndpointRouteBuilder>().MapAreaControllerRoute(name, areaName, pattern, defaults, constraints);
        // }

        public static void UseProxyToSpaDevelopmentServer(
            this ISeedSpaBuilder spaBuilder,
            string baseUri)
        {
            UseProxyToSpaDevelopmentServer(
                spaBuilder,
                new Uri(baseUri));
        }

        public static void UseProxyToSpaDevelopmentServer(
            this ISeedSpaBuilder spaBuilder,
            Uri baseUri)
        {
            UseProxyToSpaDevelopmentServer(
                spaBuilder,
                () => Task.FromResult(baseUri));
        }

        public static void UseProxyToSpaDevelopmentServer(
            this ISeedSpaBuilder spaBuilder,
            Func<Task<Uri>> baseUriTaskFactory)
        {
            var applicationBuilder = spaBuilder.ApplicationBuilder;
            var applicationStoppingToken = GetStoppingToken(applicationBuilder);
            // 用于提取path和referer里的tenantPrefix
            // var shellContext = spaBuilder.ApplicationBuilder.ApplicationServices.GetService<ShellContext>();

            // var routeBuilder = spaBuilder.ApplicationBuilder.ApplicationServices.GetService<IRouteBuilder>();
            
            
            applicationBuilder.UseWebSockets();

            var neverTimeOutHttpClient =
                SpaProxy.CreateHttpClientForProxy(Timeout.InfiniteTimeSpan);

            applicationBuilder.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value;
                var referer = context.Request.Headers["Referer"];
                var route = context.Request.RouteValues;

                var endpoint = context.GetEndpoint();

                if (endpoint != null
                    && route["action"].ToString() != "Index"
                    )
                {
                    await next();
                    return;
                }

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