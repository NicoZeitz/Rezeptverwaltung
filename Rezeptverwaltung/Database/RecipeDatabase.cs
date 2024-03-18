using Core.Entities;
using Core.Repository;
using Core.ValueObjects;

namespace Database;

public class RecipeDatabase : RecipeRepository
{
    public IEnumerable<Recipe> findForChef(Chef chef)
    {
        var reader = Database.Instance.RunQuery(@$"
            SELECT chef_id
            FROM recipe
            WHERE chef_id = {chef.Identifier.Id}
        ");

        while (reader.Read())
        {
            var chefId = new Identifier(reader.GetGuid(0));
            var preparationTime = new Duration(reader.GetTimeSpan(1));

            var recipe = new Recipe(chefId, preparationTime);
            yield return recipe;
        }

        reader.DisposeAsync();
    }
}
