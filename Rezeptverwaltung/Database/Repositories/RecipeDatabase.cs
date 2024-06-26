﻿using Core.Entities;
using Core.Interfaces;
using Core.Repository;
using Core.Services.Serialization;
using Core.ValueObjects;
using Core.ValueObjects.MeasurementUnits;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Database.Repositories;

public class RecipeDatabase : RecipeRepository
{
    private readonly MeasurementUnitSerializationManager measurementUnitManager;
    private readonly Database database;

    public RecipeDatabase(Database database, MeasurementUnitSerializationManager measurementUnitManager) : base()
    {
        this.database = database;
        this.measurementUnitManager = measurementUnitManager;
    }

    public void Add(Recipe recipe)
    {
        InsertRecipe(recipe);
        InsertTagsForRecipe(recipe);
        InsertPreparationStepsForRecipe(recipe);
        InsertWeightedIngredientsForRecipe(recipe);
    }

    public void Update(Recipe recipe)
    {
        UpdateRecipe(recipe);

        DeleteTagsForRecipe(recipe);
        DeletePreparationStepsForRecipe(recipe);
        DeleteWeightedIngredientsForRecipe(recipe);

        InsertTagsForRecipe(recipe);
        InsertPreparationStepsForRecipe(recipe);
        InsertWeightedIngredientsForRecipe(recipe);
    }

    public void Remove(Recipe recipe)
    {
        database.CreateSqlCommand(@$"
            DELETE FROM recipes
            WHERE id = {recipe.Identifier.Id};
        ").ExecuteNonQuery();
    }

    public IEnumerable<Recipe> FindAll()
    {
        var command = database.CreateSqlCommand(@$"
            SELECT
                id,
                chef,
                title,
                description,
                visibility,
                portion_numerator,
                portion_denominator,
                preparation_time
            FROM recipes;
        ");
        return GetRecipesFromSqlCommand(command);
    }

    public IEnumerable<Recipe> FindForChef(Chef chef)
    {
        var command = database.CreateSqlCommand(@$"
            SELECT
                id,
                chef,
                title,
                description,
                visibility,
                portion_numerator,
                portion_denominator,
                preparation_time
            FROM recipes
            WHERE chef = {chef.Username.Name};
        ");
        return GetRecipesFromSqlCommand(command);
    }

    public IEnumerable<Recipe> FindByTag(Tag tag)
    {
        var command = database.CreateSqlCommand(@$"
            SELECT
                id,
                chef,
                title,
                description,
                visibility,
                portion_numerator,
                portion_denominator,
                preparation_time
            FROM recipes
            WHERE id COLLATE NOCASE IN (
                SELECT recipe_id
                FROM recipe_tags
                WHERE tag_name = {tag.Text}
            );
        ");
        return GetRecipesFromSqlCommand(command);
    }

    public Recipe? FindByIdentifier(Identifier identifier)
    {
        var command = database.CreateSqlCommand(@$"
            SELECT
                id,
                chef,
                title,
                description,
                visibility,
                portion_numerator,
                portion_denominator,
                preparation_time
            FROM recipes
            WHERE id = {identifier.Id};
        ");
        return GetRecipesFromSqlCommand(command).FirstOrDefault();
    }

    public IEnumerable<Recipe> FindByTitle(Text title)
    {
        var query = $"%{title.Value.ToLower()}%";
        var command = database.CreateSqlCommand(@$"
            SELECT
                id,
                chef,
                title,
                description,
                visibility,
                portion_numerator,
                portion_denominator,
                preparation_time
            FROM recipes
            WHERE LOWER(title) LIKE {query} COLLATE NOCASE;
        ");
        return GetRecipesFromSqlCommand(command);
    }

    public IEnumerable<Recipe> FindForCookbook(Cookbook cookbook)
    {
        var command = database.CreateSqlCommand(@$"
            SELECT
                id,
                chef,
                title,
                description,
                visibility,
                portion_numerator,
                portion_denominator,
                preparation_time
            FROM recipes
            WHERE id COLLATE NOCASE IN (
                SELECT recipe_id
                FROM cookbook_recipes
                WHERE cookbook_id = {cookbook.Identifier.Id}
            );
        ");
        return GetRecipesFromSqlCommand(command);
    }

    public IEnumerable<Recipe> FindForShoppingList(ShoppingList shoppingList)
    {
        var command = database.CreateSqlCommand(@$"
            SELECT
                id,
                chef,
                title,
                description,
                visibility,
                portion_numerator,
                portion_denominator,
                preparation_time
            FROM recipes
            WHERE id COLLATE NOCASE IN (
                SELECT recipe_id
                FROM shopping_list_recipes
                WHERE shopping_list_id = {shoppingList.Identifier.Id}
            );
        ");
        return GetRecipesFromSqlCommand(command);
    }

    private IEnumerable<Recipe> GetRecipesFromSqlCommand(SqliteCommand command)
    {
        using var reader = command.ExecuteReader();
        var recipes = new List<Recipe>();

        while (reader.Read())
        {
            var recipe = CreateRecipeFromReader(reader);
            recipes.Add(recipe);
        }

        if (recipes.Count == 0)
            return recipes;

        AddRelationshipsToRecipes(recipes);
        return recipes;
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
            weightedIngredients
        );
    }

    private void InsertRecipe(Recipe recipe)
    {
        database.CreateSqlCommand(@$"
            INSERT INTO recipes (
                id,
                chef,
                title,
                description,
                visibility,
                portion_numerator,
                portion_denominator,
                preparation_time
            ) VALUES (
                {recipe.Identifier.Id},
                {recipe.Chef.Name},
                {recipe.Title.Value},
                {recipe.Description.Value},
                {recipe.Visibility},
                {recipe.Portion.Amount.Numerator},
                {recipe.Portion.Amount.Denominator},
                {recipe.PreparationTime.TimeSpan}
            );
        ").ExecuteNonQuery();
    }

    private void UpdateRecipe(Recipe recipe)
    {
        database.CreateSqlCommand(@$"
            UPDATE recipes
            SET
                chef = {recipe.Chef.Name},
                title = {recipe.Title.Value},
                description = {recipe.Description.Value},
                visibility = {recipe.Visibility},
                portion_numerator = {recipe.Portion.Amount.Numerator},
                portion_denominator = {recipe.Portion.Amount.Denominator},
                preparation_time = {recipe.PreparationTime.TimeSpan}
            WHERE id = {recipe.Identifier.Id};
        ").ExecuteNonQuery();
    }

    private void InsertTagsForRecipe(Recipe recipe)
    {
        if (recipe.Tags.Count == 0)
            return;

        database.CreateSqlCommand(@$"
            INSERT INTO tags(name)
            VALUES {recipe.Tags.Select(tag => tag.Text)}
            ON CONFLICT(name) DO NOTHING;
        ").ExecuteNonQuery();

        foreach (var tag in recipe.Tags)
        {
            database.CreateSqlCommand(@$"
                INSERT INTO recipe_tags(recipe_id, tag_name)
                VALUES ({recipe.Identifier.Id}, {tag.Text});
            ").ExecuteNonQuery();
        }
    }

    private void InsertPreparationStepsForRecipe(Recipe recipe)
    {
        for (var i = 0; i < recipe.PreparationSteps.Count; ++i)
        {
            var preparationStep = recipe.PreparationSteps[i];
            database.CreateSqlCommand(@$"
                INSERT INTO preparation_steps(recipe_id, step_number, description)
                VALUES (
                    {recipe.Identifier.Id},
                    {i},
                    {preparationStep.Description.Value}
                );
            ").ExecuteNonQuery();
        }
    }

    private void InsertWeightedIngredientsForRecipe(Recipe recipe)
    {
        foreach (var weightedIngredient in recipe.WeightedIngredients)
        {
            InsertIngredient(weightedIngredient.Ingredient);
            var measurementUnitId = InsertMeasurementUnit(weightedIngredient.PreparationQuantity);

            database.CreateSqlCommand(@$"
                INSERT INTO weighted_ingredients(recipe_id, preparation_quantity, ingredient_name)
                VALUES (
                    {recipe.Identifier.Id},
                    {measurementUnitId.Id},
                    {weightedIngredient.Ingredient.Name}
                );
            ").ExecuteNonQuery();
        }
    }

    private void InsertIngredient(Ingredient ingredient)
    {
        database.CreateSqlCommand(@$"
            INSERT INTO ingredients(name)
            VALUES ({ingredient.Name})
            ON CONFLICT(name) DO NOTHING;
        ").ExecuteNonQuery();
    }

    private Identifier InsertMeasurementUnit(MeasurementUnit measurementUnit)
    {
        var serializedMeasurementUnit = measurementUnitManager.SerializeInto(measurementUnit);
        var measurementUnitId = Identifier.NewId();
        using var reader = database.CreateSqlCommand(@$"
                INSERT INTO measurement_units(id, amount, unit)
                VALUES (
                    {measurementUnitId.Id},
                    {serializedMeasurementUnit.Amount},
                    {serializedMeasurementUnit.Unit}
                )
                ON CONFLICT(amount, unit) DO NOTHING
                RETURNING id;
            ").ExecuteReader();
        if (reader.HasRows)
        {
            reader.Read();
            measurementUnitId = new Identifier(reader.GetGuid(0));
            return measurementUnitId;
        }

        using var second_reader = database.CreateSqlCommand(@$"
            SELECT id
            FROM measurement_units
            WHERE amount = {serializedMeasurementUnit.Amount}
            AND unit = {serializedMeasurementUnit.Unit};
        ").ExecuteReader();
        second_reader.Read();
        return new Identifier(second_reader.GetGuid(0));
    }

    private void DeleteTagsForRecipe(Recipe recipe)
    {
        database.CreateSqlCommand(@$"
            DELETE FROM recipe_tags
            WHERE recipe_id = {recipe.Identifier.Id};
        ").ExecuteNonQuery();
    }

    private void DeletePreparationStepsForRecipe(Recipe recipe)
    {
        database.CreateSqlCommand(@$"
            DELETE FROM preparation_steps
            WHERE recipe_id = {recipe.Identifier.Id};
        ").ExecuteNonQuery();
    }

    private void DeleteWeightedIngredientsForRecipe(Recipe recipe)
    {
        database.CreateSqlCommand(@$"
            DELETE FROM weighted_ingredients
            WHERE recipe_id = {recipe.Identifier.Id};
        ").ExecuteNonQuery();
    }

    private void AddRelationshipsToRecipes(IList<Recipe> recipes)
    {
        if (recipes.Count == 0)
            return;

        AddTagsToRecipes(recipes);
        AddPreparationStepsToRecipes(recipes);
        AddWeightedIngredientsToRecipes(recipes);
    }

    private void AddTagsToRecipes(IList<Recipe> recipes)
    {
        var command = database.CreateSqlCommand(@$"
            SELECT
                recipe_id,
                tag_name
            FROM recipe_tags
            WHERE recipe_id COLLATE NOCASE IN ({recipes.Select(recipe => recipe.Identifier.Id.ToString())});
        ");

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var recipe = ExtractRecipeFittingToReader(reader, recipes);
            if (recipe is null)
            {
                continue;
            }

            var tag = new Tag(reader.GetString("tag_name"));

            recipe.Tags.Add(tag);
        }
    }

    private void AddPreparationStepsToRecipes(IList<Recipe> recipes)
    {
        var command = database.CreateSqlCommand(@$"
            SELECT
                recipe_id,
                step_number,
                description
            FROM preparation_steps
            WHERE recipe_id COLLATE NOCASE IN ({recipes.Select(recipe => recipe.Identifier.Id.ToString())})
            ORDER BY step_number ASC;
        ");

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var recipe = ExtractRecipeFittingToReader(reader, recipes);
            if (recipe is null)
            {
                continue;
            }

            var description = new Text(reader.GetString("description"));
            var preparationStep = new PreparationStep(description);
            recipe.PreparationSteps.Add(preparationStep);
        }
    }

    private void AddWeightedIngredientsToRecipes(IList<Recipe> recipes)
    {
        var command = database.CreateSqlCommand(@$"
            SELECT
                recipe_id,
                preparation_quantity,
                ingredient_name,
                id as measurement_unit_id,
                amount as measurement_unit_amount,
                unit as measurement_unit_unit
            FROM weighted_ingredients
            INNER JOIN measurement_units
            ON weighted_ingredients.preparation_quantity = measurement_units.id
            WHERE recipe_id COLLATE NOCASE IN ({recipes.Select(recipe => recipe.Identifier.Id.ToString())});
        ");

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var recipe = ExtractRecipeFittingToReader(reader, recipes);
            if (recipe is null)
            {
                continue;
            }

            var ingredient = new Ingredient(reader.GetString("ingredient_name"));
            var serializedMeasurementUnit = new SerializedMeasurementUnit(
                reader.GetString("measurement_unit_unit"),
                reader.GetString("measurement_unit_amount")
            );
            var measurementUnit = measurementUnitManager.DeserializeFrom(serializedMeasurementUnit);
            if (measurementUnit is null)
            {
                continue;
            }

            var weightedIngredient = new WeightedIngredient(
                measurementUnit,
                ingredient
            );

            recipe.WeightedIngredients.Add(weightedIngredient);
        }
    }

    private Recipe? ExtractRecipeFittingToReader(SqliteDataReader reader, IEnumerable<Recipe> recipes, string columnName = "recipe_id")
    {
        var recipeId = new Identifier(reader.GetGuid(columnName));
        return recipes.FirstOrDefault(recipe => recipe.Identifier == recipeId);
    }
}
