using Core.Entities;
using Core.Repository;
using Core.ValueObjects;

namespace Database.Repositories;

public class ChefDatabase : ChefRepository
{
    public ChefDatabase() : base() { }

    public void add(Chef chef)
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

    public Chef? findByName(Username username)
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

        var name = new Name(reader.GetString(0), reader.GetString(1));
        var password = new HashedPassword(reader.GetString(2));

        return new Chef(username, name, password);
    }
}
