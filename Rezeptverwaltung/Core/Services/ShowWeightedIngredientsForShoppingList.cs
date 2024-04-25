using Core.Entities;
using Core.ValueObjects;

namespace Core.Services;

public class ShowWeightedIngredientsForShoppingList
{
    private readonly AddRationalsService<int> addRationalsService;
    private readonly ShowPortionedRecipesFromShoppingList showPortionedRecipesFromShoppingList;

    public ShowWeightedIngredientsForShoppingList(
        AddRationalsService<int> addRationalsService,
        ShowPortionedRecipesFromShoppingList showPortionedRecipesFromShoppingList)
        : base()
    {
        this.addRationalsService = addRationalsService;
        this.showPortionedRecipesFromShoppingList = showPortionedRecipesFromShoppingList;
    }

    public IEnumerable<WeightedIngredient> ShowIngredients(ShoppingList shoppingList, Chef? viewer)
    {
        var recipesWithPortion = showPortionedRecipesFromShoppingList.ShowRecipes(shoppingList, viewer);
        var uniqueRecipesWithPortion = DeduplicateRecipes(recipesWithPortion);
        return SumQuantities(uniqueRecipesWithPortion);
    }

    private IEnumerable<(Portion, Recipe)> DeduplicateRecipes(IEnumerable<(Portion, Recipe)> recipesWithPortion)
    {
        var recipes = new Dictionary<Recipe, Portion>();

        foreach (var (portion, recipe) in recipesWithPortion)
        {
            if (recipes.TryGetValue(recipe, out var existingPortion))
            {
                recipes[recipe] = new Portion(addRationalsService.AddRationals(existingPortion.Amount, portion.Amount));
            }
            else
            {
                recipes[recipe] = portion;
            }
        }

        return recipesWithPortion;
    }

    private IEnumerable<WeightedIngredient> SumQuantities(IEnumerable<(Portion, Recipe)> uniqueRecipesWithPortion)
    {

        return []; // TODO:
    }
}