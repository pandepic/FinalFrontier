using ElementEngine;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Database.Tables
{
    public class UserShipComponent : Table
    {
        public string Username;
        public string ShipName;
        public ShipComponentType Slot;
        public string Seed;
        public ComponentQualityType Quality;

        public UserShipComponent() { }

        public UserShipComponent(SQLiteDataReader reader)
        {
            Username = reader["Username"].ToString();
            ShipName = reader["ShipName"].ToString();
            Slot = reader["Slot"].ToString().ToEnum<ShipComponentType>();
            Seed = reader["Seed"].ToString();
            Quality = reader["Quality"].ToString().ToEnum<ComponentQualityType>();
        }

        public override bool Insert(SQLiteCommand command)
        {
            command.CommandText = @$"
                insert into UserShipComponent (Username, ShipName, Slot, Seed, Quality)
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
            command.Parameters.AddWithValue("@Slot", Slot.ToString());
            command.Parameters.AddWithValue("@Seed", Seed);
            command.Parameters.AddWithValue("@Quality", Quality.ToString());
            command.Prepare();

            return command.ExecuteNonQuery() > 0;
        }

        public override bool Update(SQLiteCommand command)
        {
            command.CommandText = @$"
                UPDATE UserShipComponent
                SET
                    ShipName = @ShipName,
                    Slot = @Slot,
                    Seed = @Seed,
                    Quality = @Quality
                WHERE Username = @Username";

            command.Parameters.AddWithValue("@Username", Username);
            command.Parameters.AddWithValue("@ShipName", ShipName);
            command.Parameters.AddWithValue("@Slot", Slot.ToString());
            command.Parameters.AddWithValue("@Seed", Seed);
            command.Parameters.AddWithValue("@Quality", Quality.ToString());
            command.Prepare();

            return command.ExecuteNonQuery() > 0;
        }

        public static string CreateTable()
        {
            return "create table UserShipComponent (Username TEXT, ShipName TEXT, Slot TEXT, Seed TEXT, Quality TEXT)";
        }

        public static Dictionary<ShipComponentType, UserShipComponent> GetShipComponents(SQLiteCommand command, string username, string shipName)
        {
            var components = new Dictionary<ShipComponentType, UserShipComponent>();

            command.CommandText = "SELECT * FROM UserShipComponent WHERE Username = @Username AND ShipName = @ShipName";
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@ShipName", shipName);
            command.Prepare();

            using var reader = command.ExecuteReader();

            if (!reader.HasRows)
                return components;

            while (reader.Read())
            {
                var component = new UserShipComponent(reader);
                components.Add(component.Slot, component);
            }

            reader.Close();

            return components;
        }

    } // UserShipComponent
}
