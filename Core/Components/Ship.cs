using ElementEngine.ECS;
using FinalFrontier.GameData;
using FinalFrontier.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Components
{
    public struct ShipComponentSlotData
    {
        public ShipComponentType Slot;
        public string Seed;
        public ComponentQualityType Quality;
        public ShipComponentData ComponentData;
    }

    public struct ShipWeaponSlotData
    {
        public int Slot;
        public string Seed;
        public ComponentQualityType Quality;
    }

    public struct Ship
    {
        public string ShipType;
        public float MoveSpeed;
        public float TurnSpeed;
        public float TargetRotation;
        public Dictionary<ShipComponentType, ShipComponentSlotData> ShipComponentData;
        public Dictionary<int, ShipWeaponSlotData> ShipWeaponData;

        public static void WriteSync(NetworkPacket packet, Entity entity)
        {
            packet.Writer.Write((int)NetworkPacketDataType.SyncShip);
            packet.Writer.Write(entity.ID);

            ref var ship = ref entity.GetComponent<Ship>();

            packet.Writer.Write(ship.ShipType);
            packet.Writer.Write(ship.MoveSpeed);
            packet.Writer.Write(ship.TurnSpeed);
            packet.Writer.Write(ship.TargetRotation);
            packet.Writer.Write(ship.ShipComponentData.Count);

            foreach (var (slot, componentData) in ship.ShipComponentData)
            {
                packet.Writer.Write((int)slot);
                packet.Writer.Write(componentData.Seed);
                packet.Writer.Write((int)componentData.Quality);
            }

            packet.Writer.Write(ship.ShipWeaponData.Count);

            foreach (var (slot, weaponData) in ship.ShipWeaponData)
            {
                packet.Writer.Write(slot);
                packet.Writer.Write(weaponData.Seed);
                packet.Writer.Write((int)weaponData.Quality);
            }

            packet.DataCount += 1;
        }

        public static void ReadSync(Registry registry, BinaryReader reader, GameClient gameClient)
        {
            var entityID = reader.ReadInt32();

            var ship = new Ship();

            ship.ShipType = reader.ReadString();
            ship.MoveSpeed = reader.ReadSingle();
            ship.TurnSpeed = reader.ReadSingle();
            ship.TargetRotation = reader.ReadSingle();
            ship.ShipComponentData = new Dictionary<ShipComponentType, ShipComponentSlotData>();
            ship.ShipWeaponData = new Dictionary<int, ShipWeaponSlotData>();

            var componentsCount = reader.ReadInt32();

            for (var i = 0; i < componentsCount; i++)
            {
                var slot = (ShipComponentType)reader.ReadInt32();
                var seed = reader.ReadString();
                var quality = (ComponentQualityType)reader.ReadInt32();

                ship.ShipComponentData.Add(slot, new ShipComponentSlotData()
                {
                    Slot = slot,
                    Seed = seed,
                    Quality = quality,
                });
            }

            var weaponsCount = reader.ReadInt32();

            for (var i = 0; i < weaponsCount; i++)
            {
                var slot = reader.ReadInt32();
                var seed = reader.ReadString();
                var quality = (ComponentQualityType)reader.ReadInt32();

                ship.ShipWeaponData.Add(slot, new ShipWeaponSlotData()
                {
                    Slot = slot,
                    Seed = seed,
                    Quality = quality,
                });
            }

            var entity = registry.CreateEntity(entityID);
            entity.TryAddComponent(ship);
        }
    } // Ship

    public struct ShipEngine
    {
        public float BaseWarpCooldown; // in seconds
        public float WarpCooldown; // in seconds
        public bool WarpIsActive;
        public float SectorWarpSpeed;
        public float GalaxyWarpSpeed;
    }
}
