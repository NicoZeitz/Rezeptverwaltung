﻿using System.Data;
using Microsoft.Data.Sqlite;

namespace Persistence.DB;

public class Database
{
    public static Database Instance => _instance.Value;

    private static readonly Lazy<Database> _instance = new(() => new Database());
    private SqliteConnection? connection = default!;

    private Database() : base()
    {
    }

    public Database Initialize(DatabaseConfiguration configuration)
    {
        if (connection?.State == ConnectionState.Open)
            return this;

        var connectionString = new SqliteConnectionStringBuilder()
        {
            DataSource = configuration.DatabaseLocation,
            Mode = SqliteOpenMode.ReadWriteCreate,
            ForeignKeys = true,
        }.ToString();

        connection = new SqliteConnection(connectionString);

        connection.Open();

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
            visibility TEXT NOT NULL,
            preparation_time TEXT NOT NULL,
            portion_numerator INTEGER NOT NULL,
            portion_denominator INTEGER NOT NULL CHECK(portion_denominator <> 0),

            FOREIGN KEY(chef)
                REFERENCES chefs(username)
                ON UPDATE CASCADE
                ON DELETE CASCADE
        );").ExecuteNonQuery();

        CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS cookbooks (
            id TEXT PRIMARY KEY,
            title TEXT NOT NULL,
            description TEXT,
            visibility TEXT NOT NULL,
            creator TEXT NOT NULL,

            FOREIGN KEY(creator) REFERENCES chefs(username)
        );").ExecuteNonQuery();

        CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS shopping_list (
            id TEXT PRIMARY KEY,
            title TEXT NOT NULL,
            visibility TEXT NOT NULL,
            creator TEXT NOT NULL,

            FOREIGN KEY(creator) REFERENCES chefs(username)
        );").ExecuteNonQuery();

        CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS preparation_steps (
            recipe_id TEXT NOT NULL,
            step_number INTEGER NOT NULL,
            description TEXT NOT NULL,

            PRIMARY KEY(recipe_id, step_number),
            FOREIGN KEY(recipe_id)
                REFERENCES recipes(id)
                ON UPDATE CASCADE
                ON DELETE CASCADE
        );").ExecuteNonQuery();

        CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS ingredients (
            name TEXT PRIMARY KEY
        );").ExecuteNonQuery();

        CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS measurement_units (
            id TEXT PRIMARY KEY,
            discriminator TEXT NOT NULL,
            amount TEXT NOT NULL,
            unit TEXT NOT NULL,

            UNIQUE(discriminator, amount, unit)
        );").ExecuteNonQuery();

        CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS weighted_ingredients (
            recipe_id TEXT NOT NULL,
            preparation_quantity TEXT NOT NULL,
            ingredient_name TEXT NOT NULL,

            PRIMARY KEY(recipe_id, preparation_quantity, ingredient_name),
            FOREIGN KEY(recipe_id)
                REFERENCES recipes(id)
                ON UPDATE CASCADE
                ON DELETE CASCADE,
            FOREIGN KEY(preparation_quantity)
                REFERENCES measurement_units(id)
                ON UPDATE CASCADE
                ON DELETE CASCADE,
            FOREIGN KEY(ingredient_name)
                REFERENCES ingredients(name)
                ON UPDATE CASCADE
                ON DELETE CASCADE
        );").ExecuteNonQuery();

        CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS tags (
            name TEXT PRIMARY KEY
        );").ExecuteNonQuery();

        CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS recipe_tags (
            recipe_id TEXT NOT NULL,
            tag_name TEXT NOT NULL,

            PRIMARY KEY(recipe_id, tag_name),
            FOREIGN KEY(recipe_id)
                REFERENCES recipes(id)
                ON UPDATE CASCADE
                ON DELETE CASCADE,
            FOREIGN KEY(tag_name)
                REFERENCES tags(name)
                ON UPDATE CASCADE
                ON DELETE CASCADE
        );").ExecuteNonQuery();

        CreateSqlCommand(@$"CREATE TABLE IF NOT EXISTS shopping_list_recipes (
            recipe_id TEXT NOT NULL,
            shopping_list_id TEXT NOT NULL,
            portion_numerator INTEGER NOT NULL,
            portion_denominator INTEGER NOT NULL CHECK(portion_denominator <> 0),

            PRIMARY KEY(recipe_id, shopping_list_id),
            FOREIGN KEY(recipe_id)
                REFERENCES recipes(id)
                ON UPDATE CASCADE
                ON DELETE CASCADE,
            FOREIGN KEY(shopping_list_id)
                REFERENCES shopping_list(id)
                ON UPDATE CASCADE
                ON DELETE CASCADE
        );").ExecuteNonQuery();

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

        return this;
    }

    internal SqliteCommand CreateSqlCommand(QueryInterpolatedStringHandler builder)
    {
        if (connection?.State != ConnectionState.Open)
            throw new InvalidOperationException("Database connection is not open.");

        var query = builder.GetQuery();
        var parameters = builder.GetParameters();

        var command = connection.CreateCommand();

        Console.WriteLine($"Executing SQL: {query} -- Parameters: [{string.Join(", ", parameters.Select(p => $"{p.Key}='{p.Value}'"))}]");

        command.CommandText = query;
        foreach (var parameter in parameters)
        {
            command.Parameters.AddWithValue(parameter.Key, parameter.Value);
        }

        return command;
    }
}