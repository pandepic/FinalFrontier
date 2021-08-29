using ElementEngine;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Database.Tables
{
    public class User : Table
    {
        public string Username;
        public string Password;
        public uint Money;
        public string AuthToken;
        public DateTime Registered;
        public DateTime LastLogin;

        public User(SQLiteDataReader reader)
        {
            Username = reader["Username"].ToString();
            Password = reader["Password"].ToString();
            Money = reader["Money"].ConvertTo<uint>();
            AuthToken = reader["AuthToken"].ToString();
            Registered = DateTime.Parse(reader["Registered"].ToString());
            LastLogin = DateTime.Parse(reader["LastLogin"].ToString());
        }

        public override bool Insert(SQLiteCommand command)
        {
            command.CommandText = @$"
                insert into User (Username, Password, Money, AuthToken, Registered, LastLogin)
                values
                (
                    {Username},
                    {Password},
                    {Money},
                    {AuthToken},
                    {Registered.ToString(Networking.Database.DateFormatString)},
                    {LastLogin.ToString(Networking.Database.DateFormatString)},
                )";

            return command.ExecuteNonQuery() > 0;
        }

        public override bool Update(SQLiteCommand command)
        {
            command.CommandText = @$"
                UPDATE User
                SET
                    Money = {Money},
                    AuthToken = {AuthToken},
                    Registered = {Registered.ToString(Networking.Database.DateFormatString)},
                    LastLogin = {LastLogin.ToString(Networking.Database.DateFormatString)},
                WHERE Username = '{Username}'";

            return command.ExecuteNonQuery() > 0;
        }

        public static string CreateTable()
        {
            return "create table User (Username TEXT, Password TEXT, Money INTEGER, AuthToken TEXT, Registered TEXT, LastLogin TEXT)";
        }

        public static Dictionary<string, User> GetUsers(SQLiteCommand command)
        {
            var users = new Dictionary<string, User>();

            command.CommandText = "SELECT * FROM User";

            using var reader = command.ExecuteReader();

            if (!reader.HasRows)
                return users;

            while (reader.Read())
            {
                var user = new User(reader);
                users.Add(user.Username, user);
            }

            return users;
        }
    }
}
