using Core.Entities;
using Core.Repository;
using Core.ValueObjects;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Database.Repositories;

public class ShoppingListDatabase : ShoppingListRepository
{
    private readonly Database database;

    public ShoppingListDatabase(Database database)
    {
        this.database = database;
    }

    public void Add(ShoppingList shoppingList)
    {
        database.CreateSqlCommand(@$"
            INSERT INTO shopping_list (id, title, visibility, creator)
            VALUES ({shoppingList.Identifier.Id}, {shoppingList.Title.Value}, {shoppingList.Visibility}, {shoppingList.Creator.Name});
        ").ExecuteNonQuery();

        InsertPortionedRecipesForShoppingList(shoppingList);
    }

    public void Update(ShoppingList shoppingList)
    {
        database.CreateSqlCommand(@$"
            UPDATE shopping_list
            SET
                title = {shoppingList.Title.Value},
                visibility = {shoppingList.Visibility},
                creator = {shoppingList.Creator.Name}
            WHERE id = {shoppingList.Identifier.Id};
        ").ExecuteNonQuery();

        database.CreateSqlCommand(@$"
            DELETE FROM shopping_list_recipes
            WHERE shopping_list_id = {shoppingList.Identifier.Id};
        ").ExecuteNonQuery();

        DeletePortionedRecipesForShoppingList(shoppingList);
        InsertPortionedRecipesForShoppingList(shoppingList);
    }

    public void Remove(ShoppingList shoppingList)
    {
        database.CreateSqlCommand(@$"
            DELETE FROM shopping_list
            WHERE id = {shoppingList.Identifier.Id};
        ").ExecuteNonQuery();
    }

    public ShoppingList? FindByIdentifier(Identifier identifier)
    {
        var command = database.CreateSqlCommand(@$"
            SELECT
                id,
                title,
                visibility,
                creator,
                recipe_id
                portion_numerator,
                portion_denominator
            FROM shopping_list
            INNER JOIN shopping_list_recipes
            ON shopping_list.id = shopping_list_recipes.shopping_list_id
            WHERE id = {identifier.Id};
        ");
        return GetShoppingListsFromSqlCommand(command).FirstOrDefault();
    }

    public IEnumerable<ShoppingList> FindForChef(Chef chef)
    {
        var command = database.CreateSqlCommand(@$"
            SELECT
                id,
                title,
                visibility,
                creator,
                recipe_id
                portion_numerator,
                portion_denominator
            FROM shopping_list
            INNER JOIN shopping_list_recipes
            ON shopping_list.id = shopping_list_recipes.shopping_list_id
            WHERE creator = {chef.Username.Name}
            ORDER BY id;
        ");
        return GetShoppingListsFromSqlCommand(command);
    }

    private void InsertPortionedRecipesForShoppingList(ShoppingList shoppingList)
    {
        foreach (var portionedRecipe in shoppingList.PortionedRecipes)
        {
            database.CreateSqlCommand(@$"
                INSERT INTO shopping_list_recipes (recipe_id, shopping_list_id, portion_numerator, portion_denominator)
                VALUES ({portionedRecipe.RecipeIdentifier.Id}, {shoppingList.Identifier.Id}, {portionedRecipe.Portion.Amount.Numerator}, {portionedRecipe.Portion.Amount.Denominator});
            ").ExecuteNonQuery();
        }
    }

    private void DeletePortionedRecipesForShoppingList(ShoppingList shoppingList)
    {
        database.CreateSqlCommand(@$"
            DELETE FROM shopping_list_recipes
            WHERE shopping_list_id = {shoppingList.Identifier.Id};
        ").ExecuteNonQuery();
    }

    private IEnumerable<ShoppingList> GetShoppingListsFromSqlCommand(SqliteCommand command)
    {
        using var reader = command.ExecuteReader();

        ShoppingList? lastShoppingList = null;

        while (reader.Read())
        {
            var id = Identifier.Parse(reader.GetString("id"))!.Value;
            if (id == lastShoppingList?.Identifier)
            {
                var portionedRecipe = new PortionedRecipe(
                    Identifier.Parse(reader.GetString("recipe_id"))!.Value,
                    new Portion(new Rational<int>(
                        reader.GetInt32("portion_numerator"),
                        reader.GetInt32("portion_denominator")
                    ))
                );
                lastShoppingList.PortionedRecipes.Add(portionedRecipe);
                continue;
            }

            if (lastShoppingList is not null)
            {
                yield return lastShoppingList;
            }

            var title = new Text(reader.GetString("title"));
            var visibility = VisibilityExtensions.FromString(reader.GetString("visibility"));
            var creator = new Username(reader.GetString("creator"));

            lastShoppingList = new ShoppingList(
                id,
                title,
                visibility,
                creator,
                []
            );
        }

        if (lastShoppingList is not null)
        {
            yield return lastShoppingList;
        }
    }
}
