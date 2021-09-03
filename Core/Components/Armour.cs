using ElementEngine.ECS;
using FinalFrontier.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Components
{
    public struct Armour
    {
        public float BaseValue;
        public float CurrentValue;

        public static void WriteSync(NetworkPacket packet, Entity entity)
        {
            packet.Writer.Write((int)NetworkPacketDataType.SyncArmour);
            packet.Writer.Write(entity.ID);

            ref var armour = ref entity.GetComponent<Armour>();

            packet.Writer.Write(armour.BaseValue);
            packet.Writer.Write(armour.CurrentValue);

            packet.DataCount += 1;
        }

        public static void ReadSync(Registry registry, BinaryReader reader, GameClient gameClient)
        {
            var entityID = reader.ReadInt32();

            var armour = new Armour();

            armour.BaseValue = reader.ReadSingle();
            armour.CurrentValue = reader.ReadSingle();

            var entity = registry.CreateEntity(entityID);
            entity.TryAddComponent(armour);
        }
    }
}
