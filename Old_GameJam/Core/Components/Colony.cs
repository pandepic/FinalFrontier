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
    public struct Colony
    {
        public string Name;

        public static void WriteSync(NetworkPacket packet, Entity entity)
        {
            packet.Writer.Write((int)NetworkPacketDataType.SyncColony);
            packet.Writer.Write(entity.ID);
            
            ref var colony = ref entity.GetComponent<Colony>();

            packet.Writer.Write(colony.Name);

            packet.DataCount += 1;
        }

        public static void ReadSync(Registry registry, BinaryReader reader, GameClient gameClient)
        {
            var entityID = reader.ReadInt32();

            var colony = new Colony();

            colony.Name = reader.ReadString();

            var entity = registry.CreateEntity(entityID);
            entity.TryAddComponent(colony);
        }
    }
}
