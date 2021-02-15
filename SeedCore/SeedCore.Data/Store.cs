using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
// using MySqlConnector;
using OrchardCore.Environment.Shell;
using OrchardCore.Modules;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
// using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
// using Pomelo.EntityFrameworkCore.MySql.Storage;

namespace SeedCore.Data
{
    public class Store : IStore
    {
        readonly ShellSettings _settings;
        readonly IServiceProvider _serviceProvider;

        DbContextOptionsBuilder _cachedOptionsBuilder = null;
        IEnumerable<object> _cachedTypeConfigs = null;

        public Store(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _settings = serviceProvider.GetService<ShellSettings>();
        }

        public IDbContext CreateDbContext()
        {
            return CreateDbContext(_serviceProvider);
        }

        public IDbContext CreateDbContext(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                serviceProvider = _serviceProvider;
            }

            if (_cachedTypeConfigs == null)
            {
                var typeConfigs = serviceProvider.GetServices<IEntityTypeConfigurationProvider>()
                    .InvokeAsync(provider => provider.GetEntityTypeConfigurationsAsync(), null)
                    .Result;

                if (typeConfigs.Count() > 0)
                {
                    _cachedTypeConfigs = typeConfigs;
                }
            }

            var configs = _cachedTypeConfigs != null ? _cachedTypeConfigs : Enumerable.Empty<object>();

            return new ModuleDbContext(CreateOptions(configs.Count() > 0), _settings, configs);
        }

        public IDbContext CreateDbContext(IEnumerable<object> typeConfigs)
        {
            return new ModuleDbContext(CreateOptions(typeConfigs.Count() > 0), _settings, typeConfigs);
        }

        public DbContextOptions CreateOptions(bool cached = false)
        {
            if (cached && _cachedOptionsBuilder != null)
            {
                return _cachedOptionsBuilder.Options;
            }

            var optionBuilder = new DbContextOptionsBuilder();

            if (_settings["DatabaseProvider"] == null)
            {
                return null;
            }

            switch (_settings["DatabaseProvider"])
            {
                case "SqlConnection":
                    optionBuilder.UseSqlServer(_settings["ConnectionString"], ob =>
                    {
                        // ob.UseRowNumberForPaging(true);
                    });
                    break;
                case "MySql":
                    optionBuilder.UseMySql(
                        _settings["ConnectionString"],
                        ServerVersion.AutoDetect(_settings["ConnectionString"]),
                        ob =>
                        {
                            ob.CharSetBehavior(CharSetBehavior.AppendToAllColumns);
                            ob.CharSet(CharSet.Utf8Mb4);
                        });
                    break;
                default:
                    throw new ArgumentException("未知数据访问提供程序: " + _settings["DatabaseProvider"]);
            }

            optionBuilder.UseApplicationServiceProvider(_serviceProvider);

            if (cached)
            {
                optionBuilder.EnableServiceProviderCaching(true);
            }
            else
            {
                optionBuilder.EnableServiceProviderCaching(false);
            }

            if (cached && _cachedOptionsBuilder == null)
            {
                _cachedOptionsBuilder = optionBuilder;
            }

            return optionBuilder.Options;
        }
    }
}