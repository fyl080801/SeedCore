using Microsoft.Extensions.Options;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Environment.Shell.Descriptor;
using SeedCore.Environment.Shell;
using SeedCore.Environment.Shell.Data.Descriptors;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ShellSeedCoreBuilderExtensions
    {
        public static OrchardCoreBuilder AddShellDataStorage(this OrchardCoreBuilder builder)
        {
            builder.AddSitesFolder()
                .ConfigureServices(services =>
                {
                    services.AddScoped<IShellDescriptorManager, ShellDescriptorManager>();
                    services.AddScoped<IShellStateManager, ShellStateManager>();
                    services.AddScoped<IShellFeaturesManager, ShellFeaturesManager>();
                    services.AddScoped<IShellDescriptorFeaturesManager, ShellDescriptorFeaturesManager>();
                });

            return builder;
        }

        public static OrchardCoreBuilder AddSitesFolder(this OrchardCoreBuilder builder)
        {
            var services = builder.ApplicationServices;

            services.AddSingleton<IShellsSettingsSources, ShellsSettingsSources>();
            services.AddSingleton<IShellsConfigurationSources, ShellsConfigurationSources>();
            services.AddSingleton<IShellConfigurationSources, ShellConfigurationSources>();
            services.AddTransient<IConfigureOptions<ShellOptions>, ShellOptionsSetup>();
            services.AddSingleton<IShellSettingsManager, ShellSettingsManager>();

            return builder;
        }
    }
}