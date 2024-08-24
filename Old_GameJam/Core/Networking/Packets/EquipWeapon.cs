using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking.Packets
{
    public static class EquipWeaponRequest
    {
        public static void Write(NetworkPacket packet, int slot, string seed)
        {
            PacketUtil.WriteHeader(packet, NetworkPacketDataType.EquipWeapon);
            PacketUtil.WriteAuth(packet);

            packet.Writer.Write(slot);
            packet.Writer.Write(seed);
        }

        public static void Read(BinaryReader reader, out int slot, out string seed)
        {
            slot = reader.ReadInt32();
            seed = reader.ReadString();
        }
    }
}
