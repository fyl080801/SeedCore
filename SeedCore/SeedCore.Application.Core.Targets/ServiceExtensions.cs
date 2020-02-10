using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddSeedCore(this IServiceCollection services)
        {
            return AddSeedCore(services, null);
        }

        public static IServiceCollection AddSeedCore(this IServiceCollection services, Action<OrchardCoreBuilder> configure)
        {
            var builder = services.AddOrchardCore()
                .AddCommands()
                .AddMvc()
                .AddSetupFeatures("SeedModules.Setup")
                .AddDataContext()
                .AddShellDataStorage()
                .AddBackgroundService()
                .AddCaching();

            builder.ConfigureServices(s => { });

            configure?.Invoke(builder);

            return services;
        }
    }
}
