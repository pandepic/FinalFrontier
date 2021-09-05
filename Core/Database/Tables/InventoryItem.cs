using ElementEngine;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Database.Tables
{
    public class InventoryItem : Table
    {
        public string Username;
        public ShipComponentType? ComponentType; // null = weapon
        public string Seed;
        public QualityType Quality;
        public ClassType? ClassType; // used by weapons

        public InventoryItem() { }

        public InventoryItem(SQLiteDataReader reader)
        {
            var componentType = reader["ComponentType"].ToString();
            var classType = reader["ClassType"].ToString();

            Username = reader["Username"].ToString();
            ComponentType = componentType == "" ? null : componentType.ToEnum<ShipComponentType>();
            Seed = reader["Seed"].ToString();
            Quality = reader["Quality"].ToString().ToEnum<QualityType>();
            ClassType = classType == "" ? null : classType.ToEnum<ClassType>();
        }

        public override bool Insert(SQLiteCommand command)
        {
            command.CommandText = @$"
                insert into InventoryItem (Username, ComponentType, Seed, Quality, ClassType)
                values
                (
                    @Username,
                    @ComponentType,
                    @Seed,
                    @Quality,
                    @ClassType
                )";

            command.Parameters.AddWithValue("@Username", Username);
            command.Parameters.AddWithValue("@ComponentType", ComponentType.HasValue ? ComponentType.ToString() : "");
            command.Parameters.AddWithValue("@Seed", Seed);
            command.Parameters.AddWithValue("@Quality", Quality.ToString());
            command.Parameters.AddWithValue("@ClassType", ClassType.HasValue ? ClassType.ToString() : "");
            command.Prepare();

            return command.ExecuteNonQuery() > 0;
        }

        public override bool Update(SQLiteCommand command)
        {
            command.CommandText = @$"
                UPDATE InventoryItem
                SET
                    ComponentType = @ComponentType,
                    Seed = @Seed,
                    Quality = @Quality,
                    ClassType = @ClassType
                WHERE Username = @Username";

            command.Parameters.AddWithValue("@Username", Username);
            command.Parameters.AddWithValue("@ComponentType", ComponentType.HasValue ? ComponentType.ToString() : "");
            command.Parameters.AddWithValue("@Seed", Seed);
            command.Parameters.AddWithValue("@Quality", Quality.ToString());
            command.Parameters.AddWithValue("@ClassType", ClassType.HasValue ? ClassType.ToString() : "");
            command.Prepare();

            return command.ExecuteNonQuery() > 0;
        }

        public bool Remove(SQLiteCommand command)
        {
            command.CommandText = "DELETE FROM InventoryItem WHERE Username = @Username AND Seed = @Seed";
            command.Parameters.AddWithValue("@Username", Username);
            command.Parameters.AddWithValue("@Seed", Seed);
            command.Prepare();

            return command.ExecuteNonQuery() > 0;
        }

        public static string CreateTable()
        {
            return "create table InventoryItem (Username TEXT, ComponentType TEXT, Seed TEXT, Quality TEXT, ClassType TEXT)";
        }

        public static List<InventoryItem> GetItems(SQLiteCommand command, string username)
        {
            var items = new List<InventoryItem>();

            command.CommandText = "SELECT * FROM InventoryItem WHERE Username = @Username";
            command.Parameters.AddWithValue("@Username", username);
            command.Prepare();

            using var reader = command.ExecuteReader();

            if (!reader.HasRows)
                return items;

            while (reader.Read())
            {
                var item = new InventoryItem(reader);
                items.Add(item);
            }

            reader.Close();

            return items;
        }

    } // InventoryItem
}
