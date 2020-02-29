using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;
using OrchardCore.Recipes;
using OrchardCore.Settings;
using OrchardCore.Settings.Services;
using OrchardCore.Setup.Events;
using SeedCore.Settings.Services;
using SeedModules.Settings.Recipes;

namespace SeedModules.Settings
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ISetupEventHandler, SetupEventHandler>();
            services.AddRecipeExecutionStep<SettingsStep>();
            services.AddSingleton<ISiteService, SiteService>();
        }

        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
        }
    }
}
