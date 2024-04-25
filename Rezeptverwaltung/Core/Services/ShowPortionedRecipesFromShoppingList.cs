using Core.Entities;
using Core.ValueObjects;

namespace Core.Services;

public class ShowPortionedRecipesFromShoppingList
{
    private readonly ShowRecipes showRecipes;

    public ShowPortionedRecipesFromShoppingList(ShowRecipes showRecipes) : base()
    {
        this.showRecipes = showRecipes;
    }

    public IEnumerable<(Portion, Recipe)> ShowRecipes(ShoppingList shoppingList, Chef? viewer)
    {
        var recipes = showRecipes
            .ShowRecipesForShoppingList(shoppingList, viewer)
            .ToDictionary(recipe => recipe.Identifier, recipe => recipe);
        foreach (var (recipeId, portion) in shoppingList.PortionedRecipes)
        {
            if (recipes.TryGetValue(recipeId, out var recipe))
            {
                yield return (portion, recipe);
            }
        }
    }
}