using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking.Packets
{
    public static class JoinGameRequest
    {
        public static void Write(NetworkPacket packet)
        {
            PacketUtil.WriteHeader(packet, NetworkPacketDataType.JoinGame);
            PacketUtil.WriteAuth(packet);
        }
    }
}
