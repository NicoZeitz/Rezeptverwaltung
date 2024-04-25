using Core.Entities;
using Core.ValueObjects;

namespace Core.Services;

public class ShowWeightedIngredientsForShoppingList
{
    private readonly ShowPortionedRecipesFromShoppingList showPortionedRecipesFromShoppingList;

    public ShowWeightedIngredientsForShoppingList(ShowPortionedRecipesFromShoppingList showPortionedRecipesFromShoppingList)
        : base()
    {
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
                recipes[recipe] = new Portion(existingPortion.Amount + portion.Amount);
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