using ElementEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking.Packets
{
    public static class PlayerMoveToPositionRequest
    {
        public static void Write(NetworkPacket packet, Vector2 position, Vector2I sectorPosition)
        {
            PacketUtil.WriteHeader(packet, NetworkPacketDataType.PlayerMoveToPosition);
            PacketUtil.WriteAuth(packet);
            packet.Writer.Write(ref position);
            packet.Writer.Write(ref sectorPosition);
        }

        public static void Read(BinaryReader reader, out Vector2 position, out Vector2I sectorPosition)
        {
            position = reader.ReadVector2();
            sectorPosition = reader.ReadVector2I();
        }
    }
}
