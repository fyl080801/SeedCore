using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Setup.Services;
using SeedCore.Setup.Services;

namespace SeedCore.Setup
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds tenant level services.
        /// </summary>
        public static IServiceCollection AddSetup(this IServiceCollection services)
        {
            services.AddScoped<ISetupService, SetupService>();

            return services;
        }
    }
}