using System.Linq;
using OrchardCore.Modules;
using SeedCore.Data;
using SeedCore.Data.Extensions;
using SeedCore.Data.Migrations;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SeedBuilderExtensions
    {
        public static OrchardCoreBuilder AddDataContext(this OrchardCoreBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.TryAddDataProvider(name: "Microsoft SQLServer", provider: "SqlConnection");
                services.TryAddDataProvider(name: "MySql Database", provider: "MySql");

                services.AddSingleton<IStore>(sp =>
                {
                    return new Store(sp);
                });

                services.AddScoped<IDbContext>(sp =>
                {
                    var typeConfigs = sp.GetServices<IEntityTypeConfigurationProvider>()
                        .InvokeAsync(provider => provider.GetEntityTypeConfigurationsAsync(), null)
                        .Result;
                    return sp.GetService<IStore>()?.CreateDbContext(typeConfigs.ToArray());
                });
            });

            return builder;
        }
    }
}
