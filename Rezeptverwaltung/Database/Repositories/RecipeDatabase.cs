using Core.Entities;
using Core.Repository;
using Core.ValueObjects;
using Microsoft.Data.Sqlite;
using System.Data;

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
        ");

        using var reader = command.ExecuteReader();
        var recipes = new List<Recipe>();

        while (reader.Read())
        {
            var recipe = CreateRecipeFromReader(reader);
            recipes.Add(recipe);
        }

        AddRelationshipsToRecipes(recipes);

        return recipes;
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

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var chefId = new Identifier(reader.GetGuid(0));
            var preparationTime = new Duration(reader.GetTimeSpan(1));

            //var recipe = new Recipe(chefId, preparationTime);
            //yield return recipe;
        }

        return Enumerable.Empty<Recipe>();
    }


    private Recipe CreateRecipeFromReader(SqliteDataReader reader)
    {
        var recipeId = new Identifier(reader.GetGuid("id"));   
        var chefUsername = new Username(reader.GetString("chef"));
        var title = new Text(reader.GetString("title"));
        var description = new Text(reader.GetString("description"));
        var visibility = VisibilityExtensions.FromString(reader.GetString("visibility"));
        var portion = new Portion(new Rational<int>(
            reader.GetInt32("portion_numerator"), 
            reader.GetInt32("portion_denominator")
        ));
        var preparationTime = new Duration(TimeSpan.Parse(reader.GetString("preparation_time")));
        var dishImage = new Image(reader.GetStream("dish_image"));

        var tags = new List<Tag>();
        var preparationSteps = new List<PreparationStep>();
        var weightedIngredients = new List<WeightedIngredient>();

        return new Recipe(
            recipeId,
            chefUsername,
            title,
            description,
            visibility,
            portion,
            preparationTime,
            tags,
            preparationSteps,
            weightedIngredients,
            dishImage
        );
    }

    private void AddRelationshipsToRecipes(IList<Recipe> recipes)
    {
        AddTagsToRecipes(recipes);
        AddPreparationStepsToRecipes(recipes);
        AddWeightedIngredientsToRecipes(recipes);
    }

    private void AddTagsToRecipes(IList<Recipe> recipes)
    {
        var command = Database.Instance.CreateSqlCommand(@$"
            SELECT 
                recipe_id,
                tag
            FROM recipe_tags
            WHERE recipe_id IN ({recipes.Select(recipe => recipe.Identifier)})
        ");

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var recipe = GetRecipeFromReader(reader, recipes);
            if (recipe is null)
            {
                continue;
            }

            var tag = new Tag(reader.GetString("tag"));

            recipe.Tags.Add(tag);
        }
    }

    private void AddPreparationStepsToRecipes(IList<Recipe> recipes)
    {
        var command = Database.Instance.CreateSqlCommand(@$"
            SELECT 
                recipe_id,
                id,
                description,
                step_number
            FROM preparation_steps
            WHERE recipe_id IN ({recipes.Select(recipe => recipe.Identifier)})
        ");

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var recipe = GetRecipeFromReader(reader, recipes);
            if (recipe is null)
            {
                continue;
            }

            var preparationStepId = new Identifier(reader.GetGuid("id"));
            var description = new Text(reader.GetString("description"));
            var stepNumber = reader.GetInt32("step_number");

            recipe.PreparationSteps.Add(new PreparationStep(
                preparationStepId,
                description,
                recipeId,
                stepNumber
            ));
        }
    }   

    private void AddWeightedIngredientsToRecipes(IList<Recipe> recipes)
    {
        var command = Database.Instance.CreateSqlCommand(@$"
            SELECT 
                id,
                recipe_id,
                preparation_quantity,
                ingredient_name
            FROM weighted_ingredients
            WHERE recipe_id IN ({recipes.Select(recipe => recipe.Identifier)})
        ");

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var recipe = GetRecipeFromReader(reader, recipes);
            if(recipe is null)
            {
                continue;
            }

            var weightedIngredientId = new Identifier(reader.GetGuid("id"));

            var preparationQuantity = new MeasurementUnit(reader.GetString("preparation_quantity"));
            var ingredientName = new Text(reader.GetString("ingredient_name"));


            recipe.WeightedIngredients.Add(new WeightedIngredient(
                weightedIngredientId,
                preparationQuantity,
                ingredientName
            ));
        }
    }

    private Recipe? GetRecipeFromReader(SqliteDataReader reader, IEnumerable<Recipe> recipes, string columnName = "recipe_id")
    {
        var recipeId = new Identifier(reader.GetGuid(columnName));
        return recipes.FirstOrDefault(recipe => recipe.Identifier == recipeId);
    }
}
