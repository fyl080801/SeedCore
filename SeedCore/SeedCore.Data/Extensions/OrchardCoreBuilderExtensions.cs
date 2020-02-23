using System.Linq;
using OrchardCore.Modules;
using SeedCore.Data;
using SeedCore.Data.Extensions;
using SeedCore.Data.Migrations;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OrchardCoreBuilderExtensions
    {
        public static OrchardCoreBuilder AddDataContext(this OrchardCoreBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<IDataMigrationManager, DataMigrationManager>();
                services.AddScoped<IModularTenantEvents, AutoDataMigration>();
                services.AddScoped<IDataMigrator, DataMigrator>();

                services.TryAddDataProvider(name: "Microsoft SQLServer", provider: "SqlConnection");
                services.TryAddDataProvider(
                    name: "MySql Database",
                    provider: "MySql",
                    sample: "Server=localhost;Port=3306;Database=dbname;User=root;Password=;");

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
