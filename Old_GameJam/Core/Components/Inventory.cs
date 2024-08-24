using ElementEngine.ECS;
using FinalFrontier.Database.Tables;
using FinalFrontier.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Components
{
    public struct Inventory
    {
        public List<InventoryItem> Items;

        public static void WriteSync(NetworkPacket packet, Entity entity)
        {
            packet.Writer.Write((int)NetworkPacketDataType.SyncInventory);
            packet.Writer.Write(entity.ID);

            ref var inventory = ref entity.GetComponent<Inventory>();

            packet.Writer.Write(inventory.Items.Count);

            foreach (var item in inventory.Items)
            {
                packet.Writer.Write(item.ComponentType.HasValue ? (int)item.ComponentType : -1);
                packet.Writer.Write(item.Seed);
                packet.Writer.Write((int)item.Quality);
                packet.Writer.Write(item.ClassType.HasValue ? (int)item.ClassType : -1);
            }

            packet.DataCount += 1;
        } // Write

        public static void ReadSync(Registry registry, BinaryReader reader, GameClient gameClient)
        {
            var entityID = reader.ReadInt32();

            var inventory = new Inventory()
            {
                Items = new List<InventoryItem>(),
            };

            var itemCount = reader.ReadInt32();

            for (var i = 0; i < itemCount; i++)
            {
                var item = new InventoryItem();

                var componentType = reader.ReadInt32();
                item.ComponentType = componentType == -1 ? null : (ShipComponentType)componentType;

                item.Seed = reader.ReadString();
                item.Quality = (QualityType)reader.ReadInt32();

                var classType = reader.ReadInt32();
                item.ClassType = classType == -1 ? null : (ClassType)classType;

                inventory.Items.Add(item);
            }

            var entity = registry.CreateEntity(entityID);
            entity.TryAddComponent(inventory);

            if (entity.HasComponent<PlayerShip>())
            {
                ref var playerShip = ref entity.GetComponent<PlayerShip>();

                if (playerShip.Username == ClientGlobals.Username)
                    UIBuilderIngame.UpdateInventory();
            }
        } // Read
    } // Inventory
}
