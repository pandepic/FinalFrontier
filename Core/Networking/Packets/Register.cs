using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking.Packets
{
    public static class RegisterRequest
    {
        public static void Write(NetworkPacket packet, string username, string password)
        {
            PacketUtil.WriteHeader(packet, NetworkPacketDataType.Register);
            packet.Writer.Write(username);
            packet.Writer.Write(password);
        }

        public static void Read(BinaryReader reader, out string username, out string password)
        {
            username = reader.ReadString();
            password = reader.ReadString();
        }
    }

    public static class RegisterReply
    {
        public static void Write(NetworkPacket packet, string error)
        {
            PacketUtil.WriteHeader(packet, NetworkPacketDataType.Register);
            packet.Writer.Write(error);
        }

        public static void Read(BinaryReader reader, out string error)
        {
            error = reader.ReadString();
        }
    }
}
