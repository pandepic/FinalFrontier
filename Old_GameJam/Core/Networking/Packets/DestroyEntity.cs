using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking.Packets
{
    public static class DestroyEntityRequest
    {
        public static void Write(NetworkPacket packet, int id)
        {
            PacketUtil.WriteHeader(packet, NetworkPacketDataType.DestroyEntity);
            packet.Writer.Write(id);
        }

        public static void Read(BinaryReader reader, out int id)
        {
            id = reader.ReadInt32();
        }
    }
}
