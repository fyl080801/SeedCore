using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OrchardCore.Modules;
using System;

namespace SeedCore.SpaService
{
    public class Startup : StartupBase
    {
        public override int Order => -1024;
        public override int ConfigureOrder => 1024;

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISpaStaticFileProvider>(sp =>
            {
                var appContext = sp.GetRequiredService<IApplicationContext>();
                return new SpaStaticFileProvider(appContext);
            });
        }

        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            var env = serviceProvider.GetRequiredService<IHostEnvironment>();

            if (env.IsProduction())
            {
                var options = serviceProvider.GetRequiredService<IOptions<StaticFileOptions>>().Value;
                options.FileProvider = serviceProvider.GetRequiredService<ISpaStaticFileProvider>();
                app.UseStaticFiles(options);
            }
        }
    }
}