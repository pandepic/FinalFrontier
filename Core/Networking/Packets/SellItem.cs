using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking.Packets
{
    public static class SellItemRequest
    {
        public static void Write(NetworkPacket packet, string seed)
        {
            PacketUtil.WriteHeader(packet, NetworkPacketDataType.SellItem);
            PacketUtil.WriteAuth(packet);
            packet.Writer.Write(seed);
        }

        public static void Read(BinaryReader reader, out string seed)
        {
            seed = reader.ReadString();
        }
    }
}
