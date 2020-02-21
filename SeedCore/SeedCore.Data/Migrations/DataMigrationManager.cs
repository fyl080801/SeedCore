using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Descriptor.Models;
using OrchardCore.Modules;

namespace SeedCore.Data.Migrations
{
    public class DataMigrationManager : IDataMigrationManager
    {
        readonly IStore _store;
        readonly IDataMigrator _dataMigrator;
        readonly IExtensionManager _pluginManager;
        readonly IShellStateManager _engineStateManager;
        readonly ShellSettings _engineSettings;
        readonly ShellDescriptor _engineDescriptor;
        readonly IServiceProvider _serviceProvider;
        readonly ILogger _logger;

        public DataMigrationManager(
            IStore store,
            IDataMigrator dataMigrator,
            IExtensionManager pluginManager,
            IShellStateManager engineStateManager,
            ShellSettings engineSettings,
            ShellDescriptor engineDescriptor,
            IServiceProvider serviceProvider,
            ILogger<DataMigrationManager> logger)
        {
            _store = store;
            _dataMigrator = dataMigrator;
            _pluginManager = pluginManager;
            _engineStateManager = engineStateManager;
            _engineSettings = engineSettings;
            _engineDescriptor = engineDescriptor;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<string>> GetFeaturesByUpdateAsync()
        {
            return await Task.FromResult(Enumerable.Empty<string>());
        }

        public async Task Uninstall(string featureId)
        {
            await RunUpdateAsync();
        }

        public async Task UpdateAllFeaturesAsync()
        {
            await RunUpdateAsync();
        }

        public async Task UpdateAsync(string featureId)
        {
            await RunUpdateAsync();
        }

        public async Task UpdateAsync(IEnumerable<string> features)
        {
            await RunUpdateAsync();
        }

        private async Task RunUpdateAsync()
        {
            await _dataMigrator.RunAsync(await CreateDbContext());
        }

        private async Task<IDbContext> CreateDbContext()
        {
            var features = new string[0];
            try
            {
                var engineState = await _engineStateManager.GetShellStateAsync();
                features = engineState.Features.Where(e => e.IsInstalled).Select(e => e.Id).ToArray();
            }
            catch (DbException)
            {
                features = _engineDescriptor.Features.Select(e => e.Id).ToArray();
            }

            return _store.CreateDbContext(GetFeatureTypeConfigurations(features));
        }

        private async Task<IEnumerable<object>> GetFeatureTypeConfigurations(IEnumerable<string> features)
        {
            var providers = new List<IEntityTypeConfigurationProvider>();
            var providerType = typeof(IEntityTypeConfigurationProvider);
            _pluginManager.GetFeatures(features.ToArray())
                .Select(e => e.Extension)
                .Distinct()
                .Select(e =>
                {
                    return _pluginManager.LoadExtensionAsync(e).Result.ExportedTypes
                        .Where(pro => providerType.IsAssignableFrom(pro))
                        .Select(pro => ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, pro) as IEntityTypeConfigurationProvider)
                        .ToList();
                })
                .ToList()
                .ForEach(list =>
                {
                    providers = providers.Concat(list).ToList();
                });

            return await providers.Distinct().InvokeAsync(e => e.GetEntityTypeConfigurationsAsync(), _logger);
        }
    }
}