using ElementEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking.Packets
{
    public struct ServerStatusRequest : IPacketData
    {
        public static NetworkPacketDataType PacketType = NetworkPacketDataType.ServerStatus;

        public void Write(NetworkPacket packet)
        {
            packet.Writer.Write((int)PacketType);
            packet.DataCount += 1;
        }
    }

    public struct ServerStatusReply : IPacketData
    {
        public static NetworkPacketDataType PacketType = NetworkPacketDataType.ServerStatus;

        public int Players;

        public ServerStatusReply(int players)
        {
            Players = players;
        }

        public ServerStatusReply(BinaryReader reader)
        {
            Players = reader.ReadInt32();
        }

        public void Write(NetworkPacket packet)
        {
            packet.Writer.Write((int)PacketType);
            packet.Writer.Write(Players);
            packet.DataCount += 1;
        }
    }
}
