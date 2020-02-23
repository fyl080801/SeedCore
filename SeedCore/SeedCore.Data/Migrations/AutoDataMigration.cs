using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Models;
using OrchardCore.Modules;

namespace SeedCore.Data.Migrations
{
    /// <summary>
    /// Tenant 事件中自动执行数据库迁移
    /// </summary>
    public class AutoDataMigration : IModularTenantEvents
    {
        readonly ShellSettings _engineSettings;
        readonly IServiceProvider _serviceProvider;

        public AutoDataMigration(ShellSettings engineSettings, IServiceProvider serviceProvider)
        {
            _engineSettings = engineSettings;
            _serviceProvider = serviceProvider;
        }

        public Task ActivatedAsync()
        {
            if (_engineSettings.State != TenantState.Uninitialized)
            {
                return _serviceProvider.GetService<IDataMigrationManager>().UpdateAllFeaturesAsync();
            }
            return Task.CompletedTask;
        }

        public Task ActivatingAsync()
        {
            return Task.CompletedTask;
        }

        public Task TerminatedAsync()
        {
            return Task.CompletedTask;
        }

        public Task TerminatingAsync()
        {
            return Task.CompletedTask;
        }
    }
}
