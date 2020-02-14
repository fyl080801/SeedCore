using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;
using SeedCore.Setup;
using SeedCore.SpaService.Core.Extensions;

namespace SeedModules.Setup
{
    public class Startup : StartupBase
    {

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSetup();
        }

        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaControllerRoute(
                name: "Setup",
                areaName: "SeedModules.Setup",
                pattern: "",
                defaults: new { controller = "Setup", action = "Index" }
            );

            app.UseSpa(builder =>
            {
                builder.UseSeedSpaDevelopmentServer("serve");
            });
        }
    }
}
