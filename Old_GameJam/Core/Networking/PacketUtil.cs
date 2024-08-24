using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking
{
    public static class PacketUtil
    {
        public static void WriteHeader(NetworkPacket packet, NetworkPacketDataType type)
        {
            packet.Writer.Write((int)type);
            packet.DataCount += 1;
        }

        public static void WriteAuth(NetworkPacket packet)
        {
            packet.Writer.Write(ClientGlobals.Username);
            packet.Writer.Write(ClientGlobals.AuthToken);
        }

        public static void ReadAuth(BinaryReader reader, out string username, out string authToken)
        {
            username = reader.ReadString();
            authToken = reader.ReadString();
        }
    }
}
