using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking.Packets
{
    public static class LogoutRequest
    {
        public static void Write(NetworkPacket packet)
        {
            PacketUtil.WriteHeader(packet, NetworkPacketDataType.Logout);
        }
    }
}
