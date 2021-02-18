using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrchardCore.Modules;

namespace SeedModules.AdminMenu
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {

        }

        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            // if (serviceProvider.GetService<IHostEnvironment>().IsDevelopment())
            // {
            //     app.UseSpaDevelopment(builder =>
            //     {
            //         builder.Server.SuccessRegx = "Compiled successfully";
            //         builder.UseSpaDevelopmentServer("start");
            //     });
            // }
        }
    }
}
