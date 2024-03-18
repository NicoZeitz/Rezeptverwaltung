using Core.Entities;
using Microsoft.Data.Sqlite;

namespace Database
{
    public class Database
    {
        public static readonly Database Instance = new Database();

        private Database() { }

        public void Connect() { }

        internal SqliteDataReader RunQuery(QueryInterpolatedStringHandler builder)
        {
            using var connection = new SqliteConnection("");
            connection.Open();

            var query = builder.GetQuery();
            var parameters = builder.GetParameters();

            var command = connection.CreateCommand();

            command.CommandText = query;
            foreach( var parameter in parameters )
            {
                command.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            return command.ExecuteReader();
        }
       
        public static void Test()
        {
            Console.WriteLine(Recipe.Test());
        }
    }
}