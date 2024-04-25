using Core.Entities;
using Core.Repository;
using Core.ValueObjects;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Database.Repositories;

public class CookbookDatabase : CookbookRepository
{
    private readonly Database database;

    public CookbookDatabase(Database database) : base()
    {
        this.database = database;
    }

    public void Add(Cookbook cookbook)
    {
        database.CreateSqlCommand(@$"
            INSERT INTO cookbooks (id, title, description, visibility, creator)
            VALUES ({cookbook.Identifier.Id}, {cookbook.Title.Value}, {cookbook.Description.Value}, {cookbook.Visibility}, {cookbook.Creator.Name});
        ").ExecuteNonQuery();

        InsertRecipesForCookbook(cookbook);
    }

    public void Update(Cookbook cookbook)
    {
        database.CreateSqlCommand(@$"
            UPDATE cookbooks
            SET
                title = {cookbook.Title.Value},
                description = {cookbook.Description.Value},
                visibility = {cookbook.Visibility},
                creator = {cookbook.Creator.Name}
            WHERE id = {cookbook.Identifier.Id};
        ").ExecuteNonQuery();

        DeleteRecipesFromCookbook(cookbook);
        InsertRecipesForCookbook(cookbook);
    }

    public void Remove(Cookbook cookbook)
    {
        database.CreateSqlCommand(@$"
            DELETE FROM cookbooks
            WHERE id = {cookbook.Identifier.Id};
        ").ExecuteNonQuery();
    }

    public Cookbook? FindByIdentifier(Identifier identifier)
    {
        var command = database.CreateSqlCommand(@$"
            SELECT
                id,
                title,
                description,
                visibility,
                creator,
                recipe_id
            FROM cookbooks
            INNER JOIN cookbook_recipes
            ON cookbooks.id = cookbook_recipes.cookbook_id
            WHERE id = {identifier.Id};
        ");
        return GetCookbooksFromSqlCommand(command).FirstOrDefault();
    }

    public IEnumerable<Cookbook> FindForChef(Chef chef)
    {
        var command = database.CreateSqlCommand(@$"
            SELECT
                id,
                title,
                description,
                visibility,
                creator,
                recipe_id
            FROM cookbooks
            INNER JOIN cookbook_recipes
            ON cookbooks.id = cookbook_recipes.cookbook_id
            WHERE creator = {chef.Username.Name}
            ORDER BY id;
        ");
        return GetCookbooksFromSqlCommand(command);
    }

    private IEnumerable<Cookbook> GetCookbooksFromSqlCommand(SqliteCommand command)
    {
        using var reader = command.ExecuteReader();

        Cookbook? lastCookbook = null;

        while (reader.Read())
        {
            var id = Identifier.Parse(reader.GetString("id"));
            var recipeId = Identifier.Parse(reader.GetString("recipe_id"));

            if (id == lastCookbook?.Identifier)
            {
                lastCookbook.Recipes.Add(recipeId);
                continue;
            }

            if (lastCookbook is not null)
            {
                yield return lastCookbook;
            }

            var title = new Text(reader.GetString("title"));
            var description = new Text(reader.GetString("description"));
            var creator = new Username(reader.GetString("creator"));
            var visibility = Enum.Parse<Visibility>(reader.GetString("visibility"));
            lastCookbook = new Cookbook(
                id,
                title,
                description,
                creator,
                visibility,
                [recipeId]
            );
        }

        if (lastCookbook is not null)
        {
            yield return lastCookbook;
        }
    }

    private void InsertRecipesForCookbook(Cookbook cookbook)
    {
        foreach (var recipeId in cookbook.Recipes)
        {
            database.CreateSqlCommand(@$"
                INSERT INTO cookbook_recipes (cookbook_id, recipe_id)
                VALUES ({cookbook.Identifier.Id}, {recipeId.Id});
            ").ExecuteNonQuery();
        }
    }

    private void DeleteRecipesFromCookbook(Cookbook cookbook)
    {
        database.CreateSqlCommand(@$"
            DELETE FROM cookbook_recipes
            WHERE cookbook_id = {cookbook.Identifier.Id};
        ").ExecuteNonQuery();
    }
}
