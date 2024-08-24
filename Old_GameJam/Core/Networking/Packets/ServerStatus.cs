using ElementEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking.Packets
{
    public static class ServerStatusRequest
    {
        public static void Write(NetworkPacket packet)
        {
            PacketUtil.WriteHeader(packet, NetworkPacketDataType.ServerStatus);
        }
    }

    public static class ServerStatusReply
    {
        public static void Write(NetworkPacket packet, int players)
        {
            PacketUtil.WriteHeader(packet, NetworkPacketDataType.ServerStatus);
            packet.Writer.Write(players);
        }

        public static void Read(BinaryReader reader, out int players)
        {
            players = reader.ReadInt32();
        }
    }
}
