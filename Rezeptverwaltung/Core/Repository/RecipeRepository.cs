using Core.Entities;

namespace Core.Repository;

public interface RecipeRepository
{
    void add(Recipe recipe);

    IEnumerable<Recipe> findAll();

    IEnumerable<Recipe> findForChef(Chef chef);
}