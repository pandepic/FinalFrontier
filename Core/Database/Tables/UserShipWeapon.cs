using ElementEngine;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Database.Tables
{
    public class UserShipWeapon : Table
    {
        public string Username;
        public string ShipName;
        public int Slot;
        public string Seed;
        public QualityType Quality;

        public UserShipWeapon() { }

        public UserShipWeapon(SQLiteDataReader reader)
        {
            Username = reader["Username"].ToString();
            ShipName = reader["ShipName"].ToString();
            Slot = reader["Slot"].ConvertTo<int>();
            Seed = reader["Seed"].ToString();
            Quality = reader["Quality"].ToString().ToEnum<QualityType>();
        }

        public override bool Insert(SQLiteCommand command)
        {
            command.CommandText = @$"
                insert into UserShipWeapon (Username, ShipName, Slot, Seed, Quality)
                values
                (
                    @Username,
                    @ShipName,
                    @Slot,
                    @Seed,
                    @Quality
                )";

            command.Parameters.AddWithValue("@Username", Username);
            command.Parameters.AddWithValue("@ShipName", ShipName);
            command.Parameters.AddWithValue("@Slot", Slot);
            command.Parameters.AddWithValue("@Seed", Seed);
            command.Parameters.AddWithValue("@Quality", Quality.ToString());
            command.Prepare();

            return command.ExecuteNonQuery() > 0;
        }

        public override bool Update(SQLiteCommand command)
        {
            command.CommandText = @$"
                UPDATE UserShipWeapon
                SET
                    ShipName = @ShipName,
                    Slot = @Slot,
                    Seed = @Seed,
                    Quality = @Quality
                WHERE Username = @Username";

            command.Parameters.AddWithValue("@Username", Username);
            command.Parameters.AddWithValue("@ShipName", ShipName);
            command.Parameters.AddWithValue("@Slot", Slot);
            command.Parameters.AddWithValue("@Seed", Seed);
            command.Parameters.AddWithValue("@Quality", Quality.ToString());
            command.Prepare();

            return command.ExecuteNonQuery() > 0;
        }

        public static string CreateTable()
        {
            return "create table UserShipWeapon (Username TEXT, ShipName TEXT, Slot INTEGER, Seed TEXT, Quality TEXT)";
        }

        public static List<UserShipWeapon> GetShipWeapons(SQLiteCommand command, string username, string shipName)
        {
            var weapons = new List<UserShipWeapon>();

            command.CommandText = "SELECT * FROM UserShipWeapon WHERE Username = @Username AND ShipName = @ShipName";
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@ShipName", shipName);
            command.Prepare();

            using var reader = command.ExecuteReader();

            if (!reader.HasRows)
                return weapons;
            
            while (reader.Read())
            {
                var weapon = new UserShipWeapon(reader);
                weapons.Add(weapon);
            }

            reader.Close();

            return weapons;
        }

    } // UserShipWeapon
}
