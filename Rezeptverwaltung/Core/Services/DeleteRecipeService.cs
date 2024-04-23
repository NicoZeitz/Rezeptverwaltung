using Core.Entities;
using Core.Repository;
using Core.ValueObjects;

namespace Core.Services;

public class DeleteRecipeService
{
    private readonly RecipeRepository recipeRepository;

    public DeleteRecipeService(RecipeRepository recipeRepository) : base()
    {
        this.recipeRepository = recipeRepository;
    }

    public bool DeleteRecipe(Identifier id, Chef chef)
    {
        var recipe = recipeRepository.FindByIdentifier(id);
        if (recipe is null)
        {
            return false;
        }

        if (recipe.Chef != chef.Username)
        {
            return false;
        }

        recipeRepository.Remove(recipe);
        return true;
    }
}