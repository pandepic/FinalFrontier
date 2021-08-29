using FinalFrontier.Database.Tables;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking
{
    public class Database
    {
        public static readonly string DateFormatString = "YYYY-MM-dd HH:mm:ss";

        public readonly string DatabasePath = "FFDB.sqlite";

        public Dictionary<string, User> Users = new Dictionary<string, User>();

        public void Load()
        {
            if (!File.Exists(DatabasePath))
                CreateDatabase();

            using var connection = CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();

            Users = User.GetUsers(command);

            connection.Close();

        } // Load

        public void CreateDatabase()
        {
            SQLiteConnection.CreateFile(DatabasePath);

            using var connection = CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = User.CreateTable();
            command.ExecuteNonQuery();

            connection.Close();

        } // Create Database

        public SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection($"Data Source={DatabasePath};Version=3;");
        }

    } // Database
}
