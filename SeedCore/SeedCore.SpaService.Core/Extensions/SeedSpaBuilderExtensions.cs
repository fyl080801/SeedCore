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

            SpaServiceMiddleware.Attach(spaBuilder, npmScript);
        }

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

            applicationBuilder.UseWebSockets();

            var neverTimeOutHttpClient =
                SpaProxy.CreateHttpClientForProxy(Timeout.InfiniteTimeSpan);

            applicationBuilder.Use(async (context, next) =>
            {
                if (context.GetEndpoint() == null
                    && context.Request.Path.Value.StartsWith($"/{spaBuilder.Assembly.GetName().Name}"))
                {
                    await SpaProxy.PerformProxyRequest(
                        context,
                        neverTimeOutHttpClient,
                        baseUriTaskFactory(),
                        applicationStoppingToken,
                        proxy404s: true);

                    return;
                }

                await next();
            });
        }

        private static CancellationToken GetStoppingToken(IApplicationBuilder appBuilder)
        {
            var applicationLifetime = appBuilder
                .ApplicationServices
                .GetService(typeof(IHostApplicationLifetime));
            return ((IHostApplicationLifetime)applicationLifetime).ApplicationStopping;
        }
    }
}