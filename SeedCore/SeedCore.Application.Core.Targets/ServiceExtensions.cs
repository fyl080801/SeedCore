using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddSeedCoreWeb(this IServiceCollection services)
        {
            return AddSeedCoreWeb(services, null);
        }

        public static IServiceCollection AddSeedCoreWeb(this IServiceCollection services, Action<OrchardCoreBuilder> configure)
        {
            var builder = services.AddSeedCore()

                .AddCommands()
                .AddMvc()

                .AddSpa()
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
