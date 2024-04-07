using Core.Entities;
using Core.Repository;
using Core.ValueObjects;

namespace Core.Services;

public class ShowRecipes
{
    private readonly RecipeRepository recipeRepository;

    public ShowRecipes(in RecipeRepository recipeRepository) : base()
    {
        this.recipeRepository = recipeRepository;
    }

    public IEnumerable<Recipe> ShowRecipesVisibleTo(Chef? chef)
    {
        var recipes = recipeRepository.findAll();

        if (chef is null)
        {
            recipes = recipes.Where(recipe => recipe.Visibility.IsPublic());
        }
        else
        {
            recipes = recipes.Where(recipe => recipe.Visibility.IsPublic() || recipe.Chef == chef.Username);
        }

        return recipes.OrderBy(recipe => recipe.Title);
    }
}
