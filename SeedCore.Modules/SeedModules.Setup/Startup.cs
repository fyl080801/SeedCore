using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrchardCore.Modules;
using SeedCore.Setup;

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
            var env = app.ApplicationServices.GetService<IHostEnvironment>();

            routes.MapAreaControllerRoute(
                name: "Setup",
                areaName: "SeedModules.Setup",
                pattern: "",
                defaults: new { controller = "Setup", action = "Index" }
            );

            if (env.IsDevelopment())
            {
                app.UseSpaDevelopment(builder =>
                {
                    builder.Server.SuccessRegx = "Compiled successfully.";
                    builder.UseSpaDevelopmentServer("start");
                });
            }
        }
    }
}
