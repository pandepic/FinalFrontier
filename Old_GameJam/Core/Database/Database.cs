using FinalFrontier.Database.Tables;
using FinalFrontier.Utility;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking
{
    public class Database : IDisposable
    {
        public static StringCryptography StringCryptography = new StringCryptography();
        public static readonly string DateFormatString = "yyyy-MM-dd HH:mm:ss";

        public readonly string DatabasePath = "C:/FinalFrontier/FFDB.sqlite";

        public SQLiteConnection Connection;
        public Dictionary<string, User> Users = new Dictionary<string, User>();

        public void Dispose()
        {
            Unload();
        }

        public void Load()
        {
            if (!File.Exists(DatabasePath))
                CreateDatabase();

            Connection = CreateConnection();
            using var command = Connection.CreateCommand();

            Users = User.GetUsers(command);

        } // Load

        public void Unload()
        {
            Connection?.Close();
            Connection?.Dispose();
            Connection = null;
        }

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

            command.CommandText = UserShip.CreateTable();
            command.ExecuteNonQuery();

            command.CommandText = UserShipWeapon.CreateTable();
            command.ExecuteNonQuery();

            command.CommandText = UserShipComponent.CreateTable();
            command.ExecuteNonQuery();

            command.CommandText = InventoryItem.CreateTable();
            command.ExecuteNonQuery();

            var testUser = new User()
            {
                Username = "Pandepic",
                Password = "",
                Salt = "Salt",
                Money = 0,
                AuthToken = "",
                Registered = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
            };
            testUser.Password = StringCryptography.GetSaltedHashedValueAsString("password", testUser.Salt);
            testUser.Insert(command);

            var testUser2 = new User()
            {
                Username = "Pandepic2",
                Password = "",
                Salt = "Salt",
                Money = 0,
                AuthToken = "",
                Registered = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
            };
            testUser2.Password = StringCryptography.GetSaltedHashedValueAsString("password", testUser.Salt);
            testUser2.Insert(command);

            connection.Close();

        } // Create Database

        private SQLiteConnection CreateConnection()
        {
            var connection = new SQLiteConnection($"Data Source={DatabasePath};Version=3;");
            connection.Open();

            return connection;
        }

        public bool InsertUser(User user)
        {
            using var command = Connection.CreateCommand();
            var success = user.Insert(command);

            if (success)
                Users.Add(user.Username, user);

            return success;
        }

        public bool UpdateUser(User user)
        {
            using var command = Connection.CreateCommand();
            return user.Update(command);
        }

        public Dictionary<string, UserShip> GetUserShips(User user)
        {
            using var command = Connection.CreateCommand();
            return UserShip.GetShips(command, user.Username);
        }

        public Dictionary<ShipComponentType, UserShipComponent> GetShipComponents(User user, string shipName)
        {
            using var command = Connection.CreateCommand();
            return UserShipComponent.GetShipComponents(command, user.Username, shipName);
        }

        public List<UserShipWeapon> GetShipWeapons(User user, string shipName)
        {
            using var command = Connection.CreateCommand();
            return UserShipWeapon.GetShipWeapons(command, user.Username, shipName);
        }

    } // Database
}
