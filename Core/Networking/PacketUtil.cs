using System;
using System.Collections.Generic;
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
    }
}
