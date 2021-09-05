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
    public struct Shield
    {
        public float BaseValue;
        public float CurrentValue;
        public float RechargeRate;

        public Shield(ShipData shipData, ref Ship ship)
        {
            BaseValue = shipData.BaseShield;
            CurrentValue = shipData.BaseShield;
            RechargeRate = shipData.BaseShieldRegen;

            if (ship.ShipComponentData.TryGetValue(ShipComponentType.Shield, out var shield))
            {
                var shieldData = new ShipShieldData(shield);
                BaseValue *= shieldData.ShieldBonus;
                RechargeRate += shieldData.RechargeBonus;
            }
        }

        public static void WriteSync(NetworkPacket packet, Entity entity)
        {
            packet.Writer.Write((int)NetworkPacketDataType.SyncShield);
            packet.Writer.Write(entity.ID);

            ref var shield = ref entity.GetComponent<Shield>();

            packet.Writer.Write(shield.BaseValue);
            packet.Writer.Write(shield.CurrentValue);
            packet.Writer.Write(shield.RechargeRate);

            packet.DataCount += 1;
        }

        public static void ReadSync(Registry registry, BinaryReader reader, GameClient gameClient)
        {
            var entityID = reader.ReadInt32();

            var shield = new Shield();

            shield.BaseValue = reader.ReadSingle();
            shield.CurrentValue = reader.ReadSingle();
            shield.RechargeRate = reader.ReadSingle();

            var entity = registry.CreateEntity(entityID);
            entity.TryAddComponent(shield);
        }
    }
}
