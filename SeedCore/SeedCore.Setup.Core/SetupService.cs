using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Builders;
using OrchardCore.Environment.Shell.Descriptor;
using OrchardCore.Environment.Shell.Descriptor.Models;
using OrchardCore.Environment.Shell.Models;
using OrchardCore.Modules;
using OrchardCore.Recipes.Models;
using OrchardCore.Recipes.Services;
using OrchardCore.Setup.Events;
using OrchardCore.Setup.Services;
using SeedCore.Data;

namespace SeedCore.Setup.Services
{
    public class SetupService : ISetupService
    {
        private readonly IShellHost _shellHost;
        private readonly IShellContextFactory _shellContextFactory;
        private readonly IEnumerable<IRecipeHarvester> _recipeHarvesters;
        private readonly ILogger _logger;
        private readonly IStringLocalizer T;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly string _applicationName;
        private IEnumerable<RecipeDescriptor> _recipes;

        public SetupService(
            IShellHost shellHost,
            IHostEnvironment hostingEnvironment,
            IShellContextFactory shellContextFactory,
            IRunningShellTable runningShellTable,
            IEnumerable<IRecipeHarvester> recipeHarvesters,
            ILogger<SetupService> logger,
            IStringLocalizerFactory stringLocalizerFactory,
            IStringLocalizer<SetupService> stringLocalizer,
            IHostApplicationLifetime applicationLifetime
            )
        {
            _shellHost = shellHost;
            _applicationName = hostingEnvironment.ApplicationName;
            _shellContextFactory = shellContextFactory;
            _recipeHarvesters = recipeHarvesters;
            _logger = logger;
            T = stringLocalizer;
            _applicationLifetime = applicationLifetime;
        }

        public async Task<IEnumerable<RecipeDescriptor>> GetSetupRecipesAsync()
        {
            if (_recipes == null)
            {
                var recipeCollections = await Task.WhenAll(_recipeHarvesters.Select(x => x.HarvestRecipesAsync()));
                _recipes = recipeCollections.SelectMany(x => x).Where(x => x.IsSetupRecipe).ToArray();
            }

            return _recipes;
        }

        public async Task<string> SetupAsync(SetupContext context)
        {
            var initialState = context.ShellSettings.State;
            try
            {
                var executionId = await SetupInternalAsync(context);

                if (context.Errors.Any())
                {
                    context.ShellSettings.State = initialState;
                }

                return executionId;
            }
            catch
            {
                context.ShellSettings.State = initialState;
                throw;
            }
        }

        public async Task<string> SetupInternalAsync(SetupContext context)
        {
            string executionId;

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Running setup for tenant '{TenantName}'.", context.ShellSettings.Name);
            }

            string[] hardcoded =
            {
                _applicationName,
                "SeedModules.Features",
                "SeedModules.Scripting",
                "SeedModules.Recipes",
            };

            context.EnabledFeatures = hardcoded.Union(context.EnabledFeatures ?? Enumerable.Empty<string>()).Distinct().ToList();

            context.ShellSettings.State = TenantState.Initializing;

            var shellSettings = new ShellSettings(context.ShellSettings);

            if (string.IsNullOrEmpty(shellSettings["DatabaseProvider"]))
            {
                shellSettings["DatabaseProvider"] = context.DatabaseProvider;
                shellSettings["ConnectionString"] = context.DatabaseConnectionString;
                shellSettings["TablePrefix"] = context.DatabaseTablePrefix;
            }

            if (String.IsNullOrWhiteSpace(shellSettings["DatabaseProvider"]))
            {
                throw new ArgumentException($"{nameof(context.DatabaseProvider)} is required");
            }

            var shellDescriptor = new ShellDescriptor
            {
                Features = context.EnabledFeatures.Select(id => new ShellFeature { Id = id }).ToList()
            };

            using (var shellContext = await _shellContextFactory.CreateDescribedContextAsync(shellSettings, shellDescriptor))
            {
                await shellContext.CreateScope().UsingAsync(async scope =>
                {
                    IStore store;

                    try
                    {
                        store = scope.ServiceProvider.GetRequiredService<IStore>();
                        // await store.InitializeAsync(scope.ServiceProvider);
                    }
                    catch (Exception e)
                    {
                        // 表已存在或数据库问题

                        _logger.LogError(e, "An error occurred while initializing the datastore.");
                        context.Errors.Add("DatabaseProvider", T["An error occurred while initializing the datastore: {0}", e.Message]);
                        return;
                    }

                    await scope
                        .ServiceProvider
                        .GetService<IShellDescriptorManager>()
                        .UpdateShellDescriptorAsync(0,
                            shellContext.Blueprint.Descriptor.Features,
                            shellContext.Blueprint.Descriptor.Parameters);
                });

                if (context.Errors.Any())
                {
                    return null;
                }

                executionId = Guid.NewGuid().ToString("n");

                var recipeExecutor = shellContext.ServiceProvider.GetRequiredService<IRecipeExecutor>();

                await recipeExecutor.ExecuteAsync(executionId, context.Recipe, new
                {
                    SiteName = context.SiteName,
                    AdminUsername = context.AdminUsername,
                    AdminEmail = context.AdminEmail,
                    AdminPassword = context.AdminPassword,
                    DatabaseProvider = context.DatabaseProvider,
                    DatabaseConnectionString = context.DatabaseConnectionString,
                    DatabaseTablePrefix = context.DatabaseTablePrefix
                },
                _applicationLifetime.ApplicationStopping);
            }

            using (var shellContext = await _shellHost.CreateShellContextAsync(shellSettings))
            {
                await shellContext.CreateScope().UsingAsync(async scope =>
                {
                    void reportError(string key, string message)
                    {
                        context.Errors[key] = message;
                    }

                    var setupEventHandlers = scope.ServiceProvider.GetServices<ISetupEventHandler>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<SetupService>>();

                    await setupEventHandlers.InvokeAsync(x => x.Setup(
                        context.SiteName,
                        context.AdminUsername,
                        context.AdminEmail,
                        context.AdminPassword,
                        context.DatabaseProvider,
                        context.DatabaseConnectionString,
                        context.DatabaseTablePrefix,
                        context.SiteTimeZone,
                        reportError
                    ), logger);
                });

                if (context.Errors.Any())
                {
                    return executionId;
                }
            }

            shellSettings.State = TenantState.Running;
            await _shellHost.UpdateShellSettingsAsync(shellSettings);

            return executionId;
        }
    }
}