using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Shell;
using OrchardCore.Security;
using OrchardCore.Security.Permissions;
using OrchardCore.Environment.Extensions.Features;
using SeedModules.Account.Domain;

namespace SeedModules.Account.Roles
{
    public class RoleUpdater : IFeatureEventHandler
    {
        private readonly RoleManager<IRole> _roleManager;
        private readonly IEnumerable<IPermissionProvider> _permissionProviders;
        private readonly ITypeFeatureProvider _typeFeatureProvider;

        public RoleUpdater(
            RoleManager<IRole> roleManager,
            IEnumerable<IPermissionProvider> permissionProviders,
            ITypeFeatureProvider typeFeatureProvider,
            ILogger<RoleUpdater> logger)
        {
            _typeFeatureProvider = typeFeatureProvider;
            _roleManager = roleManager;
            _permissionProviders = permissionProviders;

            Logger = logger;
        }

        public ILogger Logger { get; set; }

        // void IFeatureEventHandler.Installing(IFeatureInfo feature)
        // {
        // }

        // void IFeatureEventHandler.Installed(IFeatureInfo feature)
        // {
        //     AddDefaultRolesForFeatureAsync(feature).Wait();
        // }

        // void IFeatureEventHandler.Enabling(IFeatureInfo feature)
        // {
        // }

        // void IFeatureEventHandler.Enabled(IFeatureInfo feature)
        // {
        // }

        // void IFeatureEventHandler.Disabling(IFeatureInfo feature)
        // {
        // }

        // void IFeatureEventHandler.Disabled(IFeatureInfo feature)
        // {
        // }

        // void IFeatureEventHandler.Uninstalling(IFeatureInfo feature)
        // {
        // }

        // void IFeatureEventHandler.Uninstalled(IFeatureInfo feature)
        // {
        // }

        public async Task AddDefaultRolesForFeatureAsync(IFeatureInfo feature)
        {
            var providersForEnabledModule = _permissionProviders
                .Where(x => _typeFeatureProvider.GetFeatureForDependency(x.GetType()).Id == feature.Id);

            if (Logger.IsEnabled(LogLevel.Debug))
            {
                if (providersForEnabledModule.Any())
                {
                    Logger.LogDebug("Configuring default roles for feature '{FeatureName}'", feature.Id);
                }
                else
                {
                    Logger.LogDebug("No default roles for feature '{FeatureName}'", feature.Id);
                }
            }

            foreach (var permissionProvider in providersForEnabledModule)
            {
                var stereotypes = permissionProvider.GetDefaultStereotypes();
                foreach (var stereotype in stereotypes)
                {
                    var role = await _roleManager.FindByNameAsync(stereotype.Name);
                    if (role == null)
                    {
                        if (Logger.IsEnabled(LogLevel.Information))
                        {
                            Logger.LogInformation("Defining new role '{RoleName}' for permission stereotype", stereotype.Name);
                        }

                        role = new RoleInfo { RoleName = stereotype.Name };
                        await _roleManager.CreateAsync(role);
                    }

                    var stereotypePermissionNames = (stereotype.Permissions ?? Enumerable.Empty<Permission>()).Select(x => x.Name);
                    var currentPermissionNames = ((RoleInfo)role).RoleClaims.Where(x => x.ClaimType == Permission.ClaimType).Select(x => x.ClaimValue);

                    var distinctPermissionNames = currentPermissionNames
                        .Union(stereotypePermissionNames)
                        .Distinct();

                    var additionalPermissionNames = distinctPermissionNames.Except(currentPermissionNames);

                    if (additionalPermissionNames.Any())
                    {
                        foreach (var permissionName in additionalPermissionNames)
                        {
                            if (Logger.IsEnabled(LogLevel.Debug))
                            {
                                Logger.LogDebug("Default role '{Role}' granted permission '{Permission}'", stereotype.Name, permissionName);
                            }

                            await _roleManager.AddClaimAsync(role, new Claim(Permission.ClaimType, permissionName));
                        }
                    }
                }
            }
        }

        public Task DisabledAsync(IFeatureInfo feature)
        {
            return Task.CompletedTask;
        }

        public Task DisablingAsync(IFeatureInfo feature)
        {
            return Task.CompletedTask;
        }

        public Task EnabledAsync(IFeatureInfo feature)
        {

            return Task.CompletedTask;
        }

        public Task EnablingAsync(IFeatureInfo feature)
        {
            return Task.CompletedTask;
        }

        public async Task InstalledAsync(IFeatureInfo feature)
        {
            await AddDefaultRolesForFeatureAsync(feature);
        }

        public Task InstallingAsync(IFeatureInfo feature)
        {
            return Task.CompletedTask;
        }

        public Task UninstalledAsync(IFeatureInfo feature)
        {
            return Task.CompletedTask;
        }

        public Task UninstallingAsync(IFeatureInfo feature)
        {
            return Task.CompletedTask;
        }
    }
}
