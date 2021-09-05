using ElementEngine;
using FinalFrontier.Networking.Packets;
using FinalFrontier.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking
{
    public static class ClientPacketSender
    {
        public static StringCryptography StringCryptography = new StringCryptography();

        public static void SendPacket(NetworkPacket packet)
        {
            packet.Send(GameClient.NetworkClient.Server);
        }

        public static void ServerStatus()
        {
            using var packet = new NetworkPacket();
            ServerStatusRequest.Write(packet);
            SendPacket(packet);
        }

        public static void Login(string username, string password)
        {
            using var packet = new NetworkPacket();
            LoginRequest.Write(packet, username, password);
            SendPacket(packet);
        }

        public static void Register(string username, string password)
        {
            using var packet = new NetworkPacket();
            RegisterRequest.Write(packet, username, password);
            SendPacket(packet);
        }

        public static void JoinGame()
        {
            using var packet = new NetworkPacket();
            JoinGameRequest.Write(packet);
            SendPacket(packet);
        }

        public static void PlayerMoveToPosition(Vector2 position, Vector2I sectorPosition)
        {
            using var packet = new NetworkPacket();
            PlayerMoveToPositionRequest.Write(packet, position, sectorPosition);
            SendPacket(packet);
        }

        public static void ChatMessage(string message)
        {
            using var packet = new NetworkPacket();
            ChatMessageRequest.Write(packet, message);
            SendPacket(packet);
        }

        public static void EquipComponent(ShipComponentType type, string seed)
        {
            using var packet = new NetworkPacket();
            EquipComponentRequest.Write(packet, type, seed);
            SendPacket(packet);
        }

        public static void EquipWeapon(int slot, string seed)
        {
            using var packet = new NetworkPacket();
            EquipWeaponRequest.Write(packet, slot, seed);
            SendPacket(packet);
        }

    } // ClientPackets
}
