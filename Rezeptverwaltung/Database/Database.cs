using Microsoft.Data.Sqlite;

namespace Database
{
    public class Database
    {
        public static Database Instance => _instance.Value;

        private static readonly Lazy<Database> _instance = new(() => new Database());
        private readonly SqliteConnection connection;

        private Database()
        {
            var connectionString = new SqliteConnectionStringBuilder()
            {
                DataSource = "rezeptverwaltung.db",
                Mode = SqliteOpenMode.ReadWriteCreate,
                ForeignKeys = true,
            }.ToString();

            connection = new SqliteConnection(connectionString);

            connection.Open();

            // TODO: remove https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/types
            // TODO: cascading deletes
            CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS chefs (
                username TEXT PRIMARY KEY,
                first_name TEXT,
                last_name TEXT,
                password TEXT NOT NULL CHECK(password LIKE '$argon2id$v=%')
            );").ExecuteNonQuery();

            CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS recipes (
                id TEXT PRIMARY KEY,
                chef TEXT NOT NULL,
                title TEXT NOT NULL,
                description TEXT,
                visibility TEXT NOT NULL CHECK(visibility IN ('public', 'private')),
                portion_numerator INTEGER NOT NULL,
                portion_denominator INTEGER NOT NULL CHECK(portion_denominator <> 0),
                preparation_time TEXT NOT NULL,
                dish_image BLOB,

                FOREIGN KEY(chef) REFERENCES chefs(username)
            );").ExecuteNonQuery();

            CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS cookbooks (
                id TEXT PRIMARY KEY,
                title TEXT NOT NULL,
                description TEXT,
                visibility TEXT NOT NULL CHECK(visibility IN ('public', 'private')),
                creator TEXT NOT NULL,

                FOREIGN KEY(creator) REFERENCES chefs(username)
            );").ExecuteNonQuery();

            CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS preparation_steps (
                id TEXT PRIMARY KEY,
                description TEXT NOT NULL,
                recipe_id TEXT NOT NULL,
                step_number INTEGER NOT NULL,

                UNIQUE(recipe_id, step_number),
                FOREIGN KEY(recipe_id) REFERENCES recipes(id)
            );").ExecuteNonQuery();

            CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS shopping_list (
                id TEXT PRIMARY KEY,
                title TEXT NOT NULL,
                visibility TEXT NOT NULL CHECK(visibility IN ('public', 'private')),
                creator TEXT NOT NULL,
    
                FOREIGN KEY(creator) REFERENCES chefs(username) 
            );").ExecuteNonQuery();

            CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS ingredients (
                name TEXT PRIMARY KEY
            );").ExecuteNonQuery();

            CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS measurement_units (
                id TEXT PRIMARY KEY,
                discriminator TEXT NOT NULL,
                amount BLOB NOT NULL,
                unit TEXT NOT NULL,

                UNIQUE(discriminator, amount, unit)
            );").ExecuteNonQuery();

            CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS weighted_ingredients (
                id TEXT PRIMARY KEY,
                preparation_quantity TEXT NOT NULL,
                ingredient_name TEXT NOT NULL,

                FOREIGN KEY(preparation_quantity) REFERENCES measurement_units(id),
                FOREIGN KEY(ingredient_name) REFERENCES ingredients(name)   
            );").ExecuteNonQuery();

            // m:n relationships
            CreateSqlCommand($@"CREATE TABLE IF NOT EXISTS chef_recipes (
                chef_username TEXT NOT NULL,
                recipe_id TEXT NOT NULL,

                PRIMARY KEY(chef_username, recipe_id),
                FOREIGN KEY(chef_username) 
                    REFERENCES chefs(username)
                    ON UPDATE CASCADE    
                    ON DELETE CASCADE,
                FOREIGN KEY(recipe_id) 
                    REFERENCES recipes(id)
                    ON UPDATE CASCADE   
                    ON DELETE CASCADE
            );").ExecuteNonQuery();

            CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS chef_saved_recipes (
                chef_username TEXT NOT NULL,
                recipe_id TEXT NOT NULL,

                PRIMARY KEY(chef_username, recipe_id),
                FOREIGN KEY(chef_username) 
                    REFERENCES chefs(username)
                    ON UPDATE CASCADE    
                    ON DELETE CASCADE,
                FOREIGN KEY(recipe_id) 
                    REFERENCES recipes(id)
                    ON UPDATE CASCADE    
                    ON DELETE CASCADE
            );").ExecuteNonQuery();

            CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS recipe_ingredients (
                recipe_id TEXT NOT NULL,
                ingredient_id TEXT NOT NULL,
                quantity TEXT NOT NULL,

                PRIMARY KEY(recipe_id, ingredient_id),
                FOREIGN KEY(recipe_id) 
                    REFERENCES recipes(id)
                    ON UPDATE CASCADE    
                    ON DELETE CASCADE,
                FOREIGN KEY(ingredient_id) 
                    REFERENCES ingredients(name)
                    ON UPDATE CASCADE    
                    ON DELETE CASCADE
            );").ExecuteNonQuery();

            CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS cookbook_recipes (
                cookbook_id TEXT NOT NULL,
                recipe_id TEXT NOT NULL,

                PRIMARY KEY(cookbook_id, recipe_id),
                FOREIGN KEY(cookbook_id) 
                    REFERENCES cookbooks(id)
                    ON UPDATE CASCADE
                    ON DELETE CASCADE,
                FOREIGN KEY(recipe_id) 
                    REFERENCES recipes(id)
                    ON UPDATE CASCADE    
                    ON DELETE CASCADE
            );").ExecuteNonQuery();
        }

        internal SqliteCommand CreateSqlCommand(QueryInterpolatedStringHandler builder)
        {
            var query = builder.GetQuery();
            var parameters = builder.GetParameters();

            var command = connection.CreateCommand();

            Console.WriteLine(query);

            command.CommandText = query;
            foreach (var parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            return command;
        }
    }
}