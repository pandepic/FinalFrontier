using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking.Packets
{
    public static class EquipComponentRequest
    {
        public static void Write(NetworkPacket packet, ShipComponentType type, string seed)
        {
            PacketUtil.WriteHeader(packet, NetworkPacketDataType.EquipComponent);
            PacketUtil.WriteAuth(packet);

            packet.Writer.Write((int)type);
            packet.Writer.Write(seed);
        }

        public static void Read(BinaryReader reader, out ShipComponentType type, out string seed)
        {
            type = (ShipComponentType)reader.ReadInt32();
            seed = reader.ReadString();
        }
    }
}
