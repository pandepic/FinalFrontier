using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking.Packets
{
    public static class LoginRequest
    {
        public static void Write(NetworkPacket packet, string username, string password)
        {
            PacketUtil.WriteHeader(packet, NetworkPacketDataType.Login);
            packet.Writer.Write(username);
            packet.Writer.Write(password);
        }

        public static void Read(BinaryReader reader, out string username, out string password)
        {
            username = reader.ReadString();
            password = reader.ReadString();
        }
    }

    public static class LoginReply
    {
        public static void Write(NetworkPacket packet, string authToken, string error, string worldSeed)
        {
            PacketUtil.WriteHeader(packet, NetworkPacketDataType.Login);
            packet.Writer.Write(authToken);
            packet.Writer.Write(error);
            packet.Writer.Write(worldSeed);
        }

        public static void Read(BinaryReader reader, out string authToken, out string error, out string worldSeed)
        {
            authToken = reader.ReadString();
            error = reader.ReadString();
            worldSeed = reader.ReadString();
        }
    }
}
