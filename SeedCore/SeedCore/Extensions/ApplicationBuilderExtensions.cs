using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using OrchardCore.Modules;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSeedCore(this IApplicationBuilder app, Action<IApplicationBuilder> configure = null)
        {
            app.UseOrchardCore(configure);
            app.UsePoweredBy(false, "SeedCore");

            return app;
        }
    }
}