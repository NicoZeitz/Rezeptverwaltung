using Core.Entities;
using Core.Repository;
using Core.ValueObjects;

namespace Database.Repositories;

public class RecipeDatabase : RecipeRepository
{
    public void add(Recipe recipe)
    {
        var command = Database.Instance.CreateSqlCommand(@$"
            INSERT INTO recipes (
                id,
                chef,
                title,
                description,
                visibility,
                portion_numerator,
                portion_denominator,
                preparation_time,
                dish_image
            ) VALUES (
                {recipe.Identifier},
                {recipe.Chef.Name},
                {recipe.Title},
                {recipe.Description},
                {recipe.Visibility},
                {recipe.Portion.Amount.Numerator},
                {recipe.Portion.Amount.Denominator},
                {recipe.PreparationTime},
                {recipe.DishImage}
            )
        ");

        command.ExecuteNonQuery();

        // TODO: m:n relationships
    }

    public IEnumerable<Recipe> findAll()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Recipe> findForChef(Chef chef)
    {
        var command = Database.Instance.CreateSqlCommand(@$"
            SELECT 
                id,
                chef,
                title,
                description,
                visibility,
                portion_numerator,
                portion_denominator,
                preparation_time,
                dish_image
            FROM recipes
            WHERE chef = {chef.Username.Name}
        ");

        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var chefId = new Identifier(reader.GetGuid(0));
            var preparationTime = new Duration(reader.GetTimeSpan(1));

            //var recipe = new Recipe(chefId, preparationTime);
            //yield return recipe;
        }

        reader.DisposeAsync();

        return Enumerable.Empty<Recipe>();
    }
}
