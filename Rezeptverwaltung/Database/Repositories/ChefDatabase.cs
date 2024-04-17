using Core.Entities;
using Core.Repository;
using Core.ValueObjects;
using System.Data;

namespace Database.Repositories;

public class ChefDatabase : ChefRepository
{
    public ChefDatabase() : base() { }

    public void Add(Chef chef)
    {
        var command = Database.Instance.CreateSqlCommand(@$"
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
            )
        ");
        command.ExecuteNonQuery();
    }

    public void Remove(Chef chef)
    {
        var command = Database.Instance.CreateSqlCommand(@$"
            DELETE FROM chefs
            WHERE username = {chef.Username.Name}
        ");
        command.ExecuteNonQuery();
    }

    public Chef? FindByUsername(Username username)
    {
        var command = Database.Instance.CreateSqlCommand(@$"
            SELECT first_name, last_name, password
            FROM chefs
            WHERE username = {username.Name}
        ");

        var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        var name = new Name(reader.GetString("first_name"), reader.GetString("last_name"));
        var password = new HashedPassword(reader.GetString("password"));

        return new Chef(username, name, password);
    }

}
