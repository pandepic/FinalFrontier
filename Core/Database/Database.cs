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
        public static readonly string DateFormatString = "yyyy-MM-dd HH:mm:ss";

        public readonly string DatabasePath = "C:/FinalFrontier/FFDB.sqlite";

        public Dictionary<string, User> Users = new Dictionary<string, User>();

        public void Load()
        {
            if (!File.Exists(DatabasePath))
                CreateDatabase();

            using var connection = CreateConnection();
            using var command = connection.CreateCommand();

            Users = User.GetUsers(command);

            connection.Close();

        } // Load

        public void CreateDatabase()
        {
            var fileInfo = new FileInfo(DatabasePath);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            SQLiteConnection.CreateFile(DatabasePath);

            using var connection = CreateConnection();

            using var command = connection.CreateCommand();

            command.CommandText = User.CreateTable();
            command.ExecuteNonQuery();

            connection.Close();

        } // Create Database

        public SQLiteConnection CreateConnection()
        {
            var connection = new SQLiteConnection($"Data Source={DatabasePath};Version=3;");
            connection.Open();

            return connection;
        }

    } // Database
}
