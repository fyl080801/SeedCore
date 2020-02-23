using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OrchardCore.Environment.Commands;
using OrchardCore.Recipes.Services;

namespace SeedModules.Recipes.Commands
{
    public class RecipesCommands : DefaultCommandHandler
    {
        private readonly IEnumerable<IRecipeHarvester> _recipeHarvesters;

        public RecipesCommands(
            IEnumerable<IRecipeHarvester> recipeHarvesters,
            IStringLocalizer<RecipesCommands> localizer) : base(localizer)
        {
            _recipeHarvesters = recipeHarvesters;
        }

        [CommandHelp("recipes harvest", "\tDisplays a list of available recipes.")]
        [CommandName("recipes harvest")]
        public async Task Harvest()
        {
            var recipeCollections = await Task.WhenAll(_recipeHarvesters.Select(x => x.HarvestRecipesAsync()));
            var recipes = recipeCollections.SelectMany(x => x).ToArray();

            if (!recipes.Any())
            {
                await Context.Output.WriteLineAsync(T["No recipes found."]);
                return;
            }

            await Context.Output.WriteLineAsync(T["List of available recipes"]);
            await Context.Output.WriteLineAsync(T["--------------------------"]);
            await Context.Output.WriteLineAsync();

            foreach (var recipe in recipes)
            {
                await Context.Output.WriteLineAsync(T["Recipe: {0}", recipe.Name]);
                await Context.Output.WriteLineAsync(T["  Version:     {0}", recipe.Version]);
                await Context.Output.WriteLineAsync(T["  Tags:        {0}", string.Join(",", recipe.Tags)]);
                await Context.Output.WriteLineAsync(T["  Description: {0}", recipe.Description]);
                await Context.Output.WriteLineAsync(T["  Author:      {0}", recipe.Author]);
                await Context.Output.WriteLineAsync(T["  Website:     {0}", recipe.WebSite]);
            }
        }
    }
}
