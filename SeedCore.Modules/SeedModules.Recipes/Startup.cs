using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;
using OrchardCore.Recipes;
using SeedModules.Recipes.RecipeSteps;

namespace SeedModules.Recipes
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddRecipes();

            services.AddRecipeExecutionStep<CommandStep>();
            services.AddRecipeExecutionStep<RecipesStep>();
        }
    }
}
