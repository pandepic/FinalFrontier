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
        public string Salt;
        public uint Money;
        public uint Exp;
        public RankType Rank;
        public string AuthToken;
        public DateTime Registered;
        public DateTime LastLogin;
        public bool IsBanned;

        public User() { }

        public User(SQLiteDataReader reader)
        {
            Username = reader["Username"].ToString();
            Password = reader["Password"].ToString();
            Salt = reader["Salt"].ToString();
            Money = reader["Money"].ConvertTo<uint>();
            Exp = reader["Exp"].ConvertTo<uint>();
            Rank = reader["Rank"].ToString().ToEnum<RankType>();
            AuthToken = reader["AuthToken"].ToString();
            Registered = DateTime.Parse(reader["Registered"].ToString());
            LastLogin = DateTime.Parse(reader["LastLogin"].ToString());
            IsBanned = reader["IsBanned"].ConvertTo<int>() > 0;
        }

        public override bool Insert(SQLiteCommand command)
        {
            command.CommandText = @$"
                insert into User (Username, Password, Salt, Money, Exp, Rank, AuthToken, Registered, LastLogin, IsBanned)
                values
                (
                    @Username,
                    @Password,
                    @Salt,
                    @Money,
                    @Exp,
                    @Rank,
                    @AuthToken,
                    @Registered,
                    @LastLogin,
                    @IsBanned
                )";

            command.Parameters.AddWithValue("@Username", Username);
            command.Parameters.AddWithValue("@Password", Password);
            command.Parameters.AddWithValue("@Salt", Salt);
            command.Parameters.AddWithValue("@Money", Money);
            command.Parameters.AddWithValue("@Exp", Exp);
            command.Parameters.AddWithValue("@Rank", Rank.ToString());
            command.Parameters.AddWithValue("@AuthToken", AuthToken);
            command.Parameters.AddWithValue("@Registered", Registered.ToString(Networking.Database.DateFormatString));
            command.Parameters.AddWithValue("@LastLogin", LastLogin.ToString(Networking.Database.DateFormatString));
            command.Parameters.AddWithValue("@IsBanned", IsBanned ? 1 : 0);
            command.Prepare();

            return command.ExecuteNonQuery() > 0;
        }

        public override bool Update(SQLiteCommand command)
        {
            command.CommandText = @$"
                UPDATE User
                SET
                    Money = @Money,
                    Exp = @Exp,
                    Rank = @Rank,
                    AuthToken = @AuthToken,
                    Registered = @Registered,
                    LastLogin = @LastLogin,
                    IsBanned = @IsBanned
                WHERE Username = @Username";

            command.Parameters.AddWithValue("@Username", Username);
            command.Parameters.AddWithValue("@Money", Money);
            command.Parameters.AddWithValue("@Exp", Exp);
            command.Parameters.AddWithValue("@Rank", Rank.ToString());
            command.Parameters.AddWithValue("@AuthToken", AuthToken);
            command.Parameters.AddWithValue("@Registered", Registered.ToString(Networking.Database.DateFormatString));
            command.Parameters.AddWithValue("@LastLogin", LastLogin.ToString(Networking.Database.DateFormatString));
            command.Parameters.AddWithValue("@IsBanned", IsBanned ? 1 : 0);
            command.Prepare();

            return command.ExecuteNonQuery() > 0;
        }

        public static string CreateTable()
        {
            return "create table User (Username TEXT, Password TEXT, Salt TEXT, Money INTEGER, Exp INTEGER, Rank TEXT, AuthToken TEXT, Registered TEXT, LastLogin TEXT, IsBanned INTEGER)";
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

            reader.Close();

            return users;
        }

    } // User
}
