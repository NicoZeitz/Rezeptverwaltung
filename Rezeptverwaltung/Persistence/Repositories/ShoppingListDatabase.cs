using Core.Entities;
using Core.Repository;
using Core.ValueObjects;
using Persistence.DB;
using System.Data;

namespace Persistence.Repositories;

public class ShoppingListDatabase : ShoppingListRepository
{
    private readonly Database database;

    public ShoppingListDatabase(Database database)
    {
        this.database = database;
    }

    public void Add(ShoppingList shoppingList)
    {
        var command = database.CreateSqlCommand(@$"
            INSERT INTO shopping_lists (id, title, visibility, creator)
            VALUES ({shoppingList.Identifier.Id}, {shoppingList.Title.Value}, {shoppingList.Visibility}, {shoppingList.Creator.Name});
        ");
        command.ExecuteNonQuery();

        foreach (var portionedRecipe in shoppingList.PortionedRecipes)
        {
            database.CreateSqlCommand(@$"
                INSERT INTO shopping_list_recipes (recipe_id, shopping_list_id, portion)
                VALUES ({portionedRecipe.RecipeIdentifier.Id}, {shoppingList.Identifier.Id}, {portionedRecipe.Portion.Amount});
            ").ExecuteNonQuery();
        }
    }

    public void Remove(ShoppingList shoppingList)
    {
        var command = database.CreateSqlCommand(@$"
            DELETE FROM shopping_lists
            WHERE id = {shoppingList.Identifier.Id};
        ");
        command.ExecuteNonQuery();
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
                portion
            FROM shopping_lists
            INNER JOIN shopping_list_recipes
            ON shopping_lists.id = shopping_list_recipes.shopping_list_id
            WHERE id = {identifier.Id};
        ");
        var reader = command.ExecuteReader();

        if (!reader.HasRows)
        {
            return null;
        }

        reader.Read();

        var id = Identifier.Parse(reader.GetString("id"));
        var title = new Text(reader.GetString("title"));
        var visibility = VisibilityExtensions.FromString(reader.GetString("visibility"));
        var creator = new Username(reader.GetString("creator"));

        var shoppingList = new ShoppingList(
            id,
            title,
            visibility,
            creator,
            new List<PortionedRecipe>()
        );

        do
        {
            var portionedRecipe = new PortionedRecipe(
                Identifier.Parse(reader.GetString("recipe_id")),
                new Portion(new Rational<int>(
                    reader.GetInt32("portion_numerator"),
                    reader.GetInt32("portion_denominator")
                ))
            );
            shoppingList.PortionedRecipes.Add(portionedRecipe);
        } while (reader.Read());

        return shoppingList;
    }
}
