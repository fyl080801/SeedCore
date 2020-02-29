using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Setup.Services;
using SeedModules.Setup.Services;

namespace SeedModules.Setup.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSetup(this IServiceCollection services)
        {
            services.AddScoped<ISetupService, SetupService>();

            return services;
        }
    }
}