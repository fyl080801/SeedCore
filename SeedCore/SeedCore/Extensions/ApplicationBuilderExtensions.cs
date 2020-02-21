using System;

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