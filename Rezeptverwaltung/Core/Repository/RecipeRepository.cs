using Core.Entities;

namespace Core.Repository;

public interface RecipeRepository
{
    IEnumerable<Recipe> findForChef(Chef chef);
}