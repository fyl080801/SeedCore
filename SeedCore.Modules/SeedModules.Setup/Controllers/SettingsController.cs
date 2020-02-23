using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Modules;
using OrchardCore.Setup.Services;
using SeedCore.Data;

namespace SeedModules.Setup.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ISetupService _setupService;
        private readonly IEnumerable<DatabaseProvider> _databaseProviders;
        private readonly IClock _clock;

        public SettingsController(
            ISetupService setupService,
            IEnumerable<DatabaseProvider> databaseProviders,
            IClock clock)
        {
            _setupService = setupService;
            _databaseProviders = databaseProviders;
            _clock = clock;
        }

        [HttpGet]
        public IActionResult DatabaseProviders()
        {
            return Json(_databaseProviders);
        }

        [HttpGet]
        public IActionResult TimeZones()
        {
            return Json(_clock.GetTimeZones()
                .Select(e => new { e.TimeZoneId, TimeZoneName = e.ToString() })
                .ToArray());
        }

        [HttpGet]
        public async Task<IActionResult> Recipes()
        {
            var recipes = await _setupService.GetSetupRecipesAsync();
            return Json(recipes
                .Where(e => e.IsSetupRecipe)
                .Select(e => new
                {
                    e.Author,
                    e.BasePath,
                    e.Description,
                    e.DisplayName,
                    e.IsSetupRecipe,
                    e.Name,
                    e.Version,
                    e.Tags,
                    e.WebSite
                })
                .ToArray());
        }
    }
}