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
        var command = database.CreateSqlCommand(@$"
            INSERT INTO cookbooks (id, title, description, visibility, creator)
            VALUES ({cookbook.Identifier.Id}, {cookbook.Title.Value}, {cookbook.Description.Value}, {cookbook.Visibility}, {cookbook.Creator.Name});
        ");
        command.ExecuteNonQuery();

        foreach (var recipeId in cookbook.Recipes)
        {
            database.CreateSqlCommand(@$"
                INSERT INTO cookbook_recipes (cookbook_id, recipe_id)
                VALUES ({cookbook.Identifier.Id}, {recipeId.Id});
            ").ExecuteNonQuery();
        }
    }

    public void Remove(Cookbook cookbook)
    {
        var command = database.CreateSqlCommand(@$"
            DELETE FROM cookbooks
            WHERE id = {cookbook.Identifier.Id};
        ");
        command.ExecuteNonQuery();
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
        return GetSingleCookbookFromSqlCommand(command);
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
        ");
        return GetCookbooksFromSqlCommand(command);
    }

    public IEnumerable<Cookbook> GetCookbooksFromSqlCommand(SqliteCommand command)
    {
        var reader = command.ExecuteReader();
        var cookbooks = new Dictionary<string, Cookbook>();

        while (reader.Read())
        {
            var cookbookId = reader.GetString("id");
            var recipeId = Identifier.Parse(reader.GetString("recipe_id"));

            if (cookbooks.TryGetValue(cookbookId, out var existingCookbook))
            {
                existingCookbook.Recipes.Add(recipeId);
                continue;
            }

            var title = new Text(reader.GetString("title"));
            var description = new Text(reader.GetString("description"));
            var creator = new Username(reader.GetString("creator"));
            var visibility = VisibilityExtensions.FromString(reader.GetString("visibility"));
            var cookbook = new Cookbook(
                Identifier.Parse(cookbookId),
                title,
                description,
                creator,
                visibility,
                new[] { recipeId }
            );
            cookbooks.Add(cookbookId, cookbook);
        }

        return cookbooks.Select(tuple => tuple.Value);
    }

    public Cookbook? GetSingleCookbookFromSqlCommand(SqliteCommand command)
    {
        var reader = command.ExecuteReader();

        if (!reader.HasRows)
        {
            return null;
        }

        reader.Read();

        var id = Identifier.Parse(reader.GetString("id"));
        var title = new Text(reader.GetString("title"));
        var description = new Text(reader.GetString("description"));
        var visibility = Enum.Parse<Visibility>(reader.GetString("visibility"));
        var creator = new Username(reader.GetString("creator"));
        var recipes = new List<Identifier>();

        do
        {
            var recipeId = Identifier.Parse(reader.GetString("recipe_id"));
            recipes.Add(recipeId);
        } while (reader.Read());

        return new Cookbook(id, title, description, creator, visibility, recipes);
    }
}
