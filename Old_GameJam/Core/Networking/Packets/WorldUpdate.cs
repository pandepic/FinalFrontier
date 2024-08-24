using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking.Packets
{
    public static class WorldUpdateReply
    {
        public static void Write(NetworkPacket packet, double worldTime)
        {
            PacketUtil.WriteHeader(packet, NetworkPacketDataType.WorldUpdate);
            packet.Writer.Write(worldTime);
        }

        public static void Read(BinaryReader reader, out double worldTime)
        {
            worldTime = reader.ReadDouble();
        }
    }
}
