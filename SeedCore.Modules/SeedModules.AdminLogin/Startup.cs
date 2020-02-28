using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrchardCore.Modules;
using System;

namespace SeedModules.Setup
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {

        }

        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            var env = app.ApplicationServices.GetService<IHostEnvironment>();

            routes.MapAreaControllerRoute(
                name: "AdminLogin",
                areaName: "SeedModules.AdminLogin",
                pattern: "",
                defaults: new { controller = "Admin", action = "Login" }
            );

            if (env.IsDevelopment())
            {
                app.UseSpaDevelopment(builder =>
                {
                    builder.Server.SuccessRegx = "Project is running at";
                    builder.UseSpaDevelopmentServer("start");
                });
            }
        }
    }
}
