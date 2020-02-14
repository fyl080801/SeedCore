using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Modules;
using SeedCore.Setup;
using SeedModules.Setup.Middleware;
using SeedModules.Setup.Extensions;

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
