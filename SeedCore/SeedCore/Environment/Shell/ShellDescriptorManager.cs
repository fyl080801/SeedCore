using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Environment.Shell.Descriptor;
using OrchardCore.Environment.Shell.Descriptor.Models;
using OrchardCore.Modules;
using SeedCore.Data;

namespace SeedCore.Environment.Shell.Data.Descriptors
{
    public class ShellDescriptorManager : IShellDescriptorManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ShellSettings _shellSettings;
        private readonly IShellConfiguration _shellConfiguration;
        private readonly IEnumerable<ShellFeature> _alwaysEnabledFeatures;
        private readonly IEnumerable<IShellDescriptorManagerEventHandler> _shellDescriptorManagerEventHandlers;
        private readonly IDbContext _context;
        private readonly ILogger _logger;
        private ShellDescriptor _shellDescriptor;

        public ShellDescriptorManager(
            IServiceProvider serviceProvider,
            ShellSettings shellSettings,
            IShellConfiguration shellConfiguration,
            IEnumerable<ShellFeature> shellFeatures,
            IEnumerable<IShellDescriptorManagerEventHandler> shellDescriptorManagerEventHandlers,
            IDbContext context,
            ILogger<ShellDescriptorManager> logger)
        {
            _serviceProvider = serviceProvider;
            _shellSettings = shellSettings;
            _shellConfiguration = shellConfiguration;
            _alwaysEnabledFeatures = shellFeatures.Where(f => f.AlwaysEnabled).ToArray();
            _shellDescriptorManagerEventHandlers = shellDescriptorManagerEventHandlers;
            _context = context;
            _logger = logger;
        }

        public async Task<ShellDescriptor> GetShellDescriptorAsync()
        {
            // Prevent multiple queries during the same request
            if (_shellDescriptor == null)
            {
                _shellDescriptor = await new ValueTask<ShellDescriptor>(_context.Set<ShellDescriptor>().FirstOrDefault());

                if (_shellDescriptor == null)
                {
                    _shellDescriptor = await new ValueTask<ShellDescriptor>(_context.Set<ShellDescriptor>().FirstOrDefault());
                }

                if (_shellDescriptor != null)
                {
                    var configuredFeatures = new ConfiguredFeatures();
                    _shellConfiguration.Bind(configuredFeatures);

                    var features = _alwaysEnabledFeatures.Concat(configuredFeatures.Features
                        .Select(id => new ShellFeature(id) { AlwaysEnabled = true })).Distinct();

                    _shellDescriptor.Features = features
                        .Concat(_shellDescriptor.Features)
                        .Distinct()
                        .ToList();
                }
            }

            return _shellDescriptor;
        }

        public async Task UpdateShellDescriptorAsync(int priorSerialNumber, IEnumerable<ShellFeature> enabledFeatures, IEnumerable<ShellParameter> parameters)
        {
            var shellDescriptorRecord = await GetShellDescriptorAsync();
            var serialNumber = shellDescriptorRecord == null
                ? 0
                : shellDescriptorRecord.SerialNumber;

            if (priorSerialNumber != serialNumber)
            {
                throw new InvalidOperationException("Invalid serial number for shell descriptor");
            }

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Updating shell descriptor for tenant '{TenantName}' ...", _shellSettings.Name);
            }

            if (shellDescriptorRecord == null)
            {
                shellDescriptorRecord = new ShellDescriptor { SerialNumber = 1 };
            }
            else
            {
                shellDescriptorRecord.SerialNumber++;
            }

            shellDescriptorRecord.Features = _alwaysEnabledFeatures.Concat(enabledFeatures).Distinct().ToList();
            shellDescriptorRecord.Parameters = parameters.ToList();

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Shell descriptor updated for tenant '{TenantName}'.", _shellSettings.Name);
            }

            var dbset = _context.Set<ShellDescriptor>();
            var exisit = await dbset.FindAsync(shellDescriptorRecord.SerialNumber);

            if (exisit != null)
            {
                dbset.Update(shellDescriptorRecord);
                _context.SaveChanges();
            }
            else
            {
                dbset.Add(shellDescriptorRecord);
            }

            // Update cached reference
            _shellDescriptor = shellDescriptorRecord;

            await _shellDescriptorManagerEventHandlers.InvokeAsync((handler, shellDescriptorRecord, _shellSettings) =>
                handler.ChangedAsync(shellDescriptorRecord, _shellSettings), shellDescriptorRecord, _shellSettings, _logger);
        }

        private class ConfiguredFeatures
        {
            public string[] Features { get; set; } = Array.Empty<string>();
        }
    }
}
