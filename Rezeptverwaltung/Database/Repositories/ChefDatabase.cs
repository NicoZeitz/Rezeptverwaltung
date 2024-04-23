using Core.Entities;
using Core.Repository;
using Core.ValueObjects;
using System.Data;

namespace Database.Repositories;

public class ChefDatabase : ChefRepository
{
    private readonly Database database;

    public ChefDatabase(Database database) : base()
    {
        this.database = database;
    }

    public void Add(Chef chef)
    {
        database.CreateSqlCommand(@$"
            INSERT INTO chefs (
                username,
                first_name,
                last_name,
                password
            ) VALUES (
                {chef.Username.Name},
                {chef.Name.FirstName},
                {chef.Name.LastName},
                {chef.HashedPassword.Hash}
            );
        ").ExecuteNonQuery();
    }

    public void Update(Chef chef)
    {
        database.CreateSqlCommand(@$"
            UPDATE chefs
            SET
                first_name = {chef.Name.FirstName},
                last_name = {chef.Name.LastName},
                password = {chef.HashedPassword.Hash}
            WHERE username = {chef.Username.Name};
        ").ExecuteNonQuery();
    }

    public void Remove(Chef chef)
    {
        database.CreateSqlCommand(@$"
            DELETE FROM chefs
            WHERE username = {chef.Username.Name};
        ").ExecuteNonQuery();
    }

    public Chef? FindByUsername(Username username)
    {
        var command = database.CreateSqlCommand(@$"
            SELECT first_name, last_name, password
            FROM chefs
            WHERE username = {username.Name};
        ");

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        var name = new Name(reader.GetString("first_name"), reader.GetString("last_name"));
        var password = new HashedPassword(reader.GetString("password"));

        return new Chef(username, name, password);
    }

}
