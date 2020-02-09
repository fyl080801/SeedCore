using System;
using Microsoft.AspNetCore.Builder;
using OrchardCore.Modules;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SeedModules.Features.Recipes.Executors;
using OrchardCore.Features.Services;
using OrchardCore.Recipes;
using SeedModules.Features.Services;

namespace OrchardCore.Features
{
    /// <summary>
    /// These services are registered on the tenant service collection
    /// </summary>
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddRecipeExecutionStep<FeatureStep>();
            // services.AddScoped<IPermissionProvider, Permissions>();
            services.AddScoped<IModuleService, ModuleService>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
        }
    }
}
