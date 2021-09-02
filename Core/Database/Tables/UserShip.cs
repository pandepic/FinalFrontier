using ElementEngine;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Database.Tables
{
    public class UserShip : Table
    {
        public string Username;
        public string ShipName;
        public bool IsActive;

        public List<UserShipComponent> Components = new List<UserShipComponent>();
        public List<UserShipWeapon> Weapons = new List<UserShipWeapon>();

        public UserShip() { }

        public UserShip(SQLiteDataReader reader)
        {
            Username = reader["Username"].ToString();
            ShipName = reader["ShipName"].ToString();
            IsActive = reader["IsActive"].ConvertTo<int>() > 0;
        }
        
        public override bool Insert(SQLiteCommand command)
        {
            command.CommandText = @$"
                insert into UserShip (Username, ShipName, IsActive)
                values
                (
                    @Username,
                    @ShipName,
                    @IsActive
                )";

            command.Parameters.AddWithValue("@Username", Username);
            command.Parameters.AddWithValue("@ShipName", ShipName);
            command.Parameters.AddWithValue("@IsActive", IsActive ? 1 : 0);
            command.Prepare();

            var inserted = command.ExecuteNonQuery() > 0;

            if (inserted)
            {
                ClearComponents(command);
                ClearWeapons(command);
                SaveComponents(command);
                SaveWeapons(command);
            }

            return inserted;
        }

        public override bool Update(SQLiteCommand command)
        {
            command.CommandText = @$"
                UPDATE UserShip
                SET
                    ShipName = @ShipName,
                    IsActive = @IsActive
                WHERE Username = @Username";

            command.Parameters.AddWithValue("@Username", Username);
            command.Parameters.AddWithValue("@ShipName", ShipName);
            command.Parameters.AddWithValue("@IsActive", IsActive ? 1 : 0);
            command.Prepare();

            var updated = command.ExecuteNonQuery() > 0;

            if (updated)
            {
                ClearComponents(command);
                ClearWeapons(command);
                SaveComponents(command);
                SaveWeapons(command);
            }

            return updated;
        }

        public bool ClearComponents(SQLiteCommand command)
        {
            command.CommandText = "DELETE FROM UserShipComponent WHERE Username = @Username AND ShipName = @ShipName";
            command.Parameters.AddWithValue("@Username", Username);
            command.Parameters.AddWithValue("@ShipName", ShipName);
            command.Prepare();
            var cleared = command.ExecuteNonQuery();
            return cleared > 0;
        }

        public bool ClearWeapons(SQLiteCommand command)
        {
            command.CommandText = "DELETE FROM UserShipWeapon WHERE Username = @Username AND ShipName = @ShipName";
            command.Parameters.AddWithValue("@Username", Username);
            command.Parameters.AddWithValue("@ShipName", ShipName);
            command.Prepare();
            return command.ExecuteNonQuery() > 0;
        }

        public void SaveComponents(SQLiteCommand command)
        {
            foreach (var component in Components)
                component.Insert(command);
        }

        public void SaveWeapons(SQLiteCommand command)
        {
            foreach (var weapon in Weapons)
                weapon.Insert(command);
        }

        public static string CreateTable()
        {
            return "create table UserShip (Username TEXT, ShipName TEXT, IsActive INTEGER)";
        }

        public static Dictionary<string, UserShip> GetShips(SQLiteCommand command, string username)
        {
            var ships = new Dictionary<string, UserShip>();
            
            command.CommandText = "SELECT * FROM UserShip WHERE Username = @Username";
            command.Parameters.AddWithValue("@Username", username);
            command.Prepare();

            using var reader = command.ExecuteReader();

            if (!reader.HasRows)
                return ships;

            while (reader.Read())
            {
                var ship = new UserShip(reader);
                ships.Add(ship.ShipName, ship);
            }

            reader.Close();

            foreach (var (_, ship) in ships)
            {
                var components = UserShipComponent.GetShipComponents(command, username, ship.ShipName);
                var weapons = UserShipWeapon.GetShipWeapons(command, username, ship.ShipName);

                foreach (var (_, component) in components)
                    ship.Components.Add(component);

                foreach (var weapon in weapons)
                    ship.Weapons.Add(weapon);
            }

            return ships;
        }

    } // UserShip
}
