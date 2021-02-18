using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrchardCore.Modules;
using SeedCore.Data;

namespace SeedModules.Admin
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IEntityTypeConfigurationProvider, EntityTypeConfigurationProvider>();
        }

        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            // routes.MapAreaControllerRoute("Admin", "SeedModules.Admin", "", new { controller = "Home", Action = "Index" });

            // if (serviceProvider.GetService<IHostEnvironment>().IsDevelopment())
            // {
            //     app.UseSpaDevelopment(builder =>
            //     {
            //         builder.Server.SuccessRegx = "Project is running at";
            //         builder.UseSpaDevelopmentServer("start");
            //     });
            // }
        }
    }
}
