using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking.Packets
{
    class ChatMessage
    {
    }

    public static class ChatMessageRequest
    {
        public static void Write(NetworkPacket packet, string message)
        {
            PacketUtil.WriteHeader(packet, NetworkPacketDataType.ChatMessage);
            PacketUtil.WriteAuth(packet);
            packet.Writer.Write(message);
        }

        public static void Read(BinaryReader reader, out string message)
        {
            message = reader.ReadString();
        }
    }

    public static class ChatMessageReply
    {
        public static void Write(NetworkPacket packet, string message)
        {
            PacketUtil.WriteHeader(packet, NetworkPacketDataType.ChatMessage);
            packet.Writer.Write(message);
        }

        public static void Read(BinaryReader reader, out string message)
        {
            message = reader.ReadString();
        }
    }
}
