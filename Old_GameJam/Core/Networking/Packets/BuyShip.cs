using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking.Packets
{
    public static class BuyShipRequest
    {
        public static void Write(NetworkPacket packet, string name)
        {
            PacketUtil.WriteHeader(packet, NetworkPacketDataType.BuyShip);
            PacketUtil.WriteAuth(packet);
            packet.Writer.Write(name);
        }

        public static void Read(BinaryReader reader, out string name)
        {
            name = reader.ReadString();
        }
    }
}
