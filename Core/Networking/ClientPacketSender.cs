using ElementEngine;
using FinalFrontier.Networking.Packets;
using FinalFrontier.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var salt = SettingsManager.GetSetting<string>("Account", "Salt");
            var hashedPassword = Encoding.UTF8.GetString(StringCryptography.GetSaltedHashedValue(password, salt));

            using var packet = new NetworkPacket();
            LoginRequest.Write(packet, username, hashedPassword);
            SendPacket(packet);
        }

        public static void Register(string username, string password)
        {
            var salt = SettingsManager.GetSetting<string>("Account", "Salt");
            var hashedPassword = Encoding.UTF8.GetString(StringCryptography.GetSaltedHashedValue(password, salt));

            using var packet = new NetworkPacket();
            RegisterRequest.Write(packet, username, hashedPassword);
            SendPacket(packet);
        }
        
    } // ClientPackets
}
