using ElementEngine;
using FinalFrontier.Database.Tables;
using FinalFrontier.Networking.Packets;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking
{
    public partial class NetworkServer
    {
        public void HandleLogin(BinaryReader reader, NetPeer peer)
        {
            LoginRequest.Read(reader, out var username, out var password);

            if (!Database.Users.TryGetValue(username, out var user))
            {
                using var packetError = new NetworkPacket();
                LoginReply.Write(packetError, "", "InvalidUsernamePassword", "");
                packetError.Send(peer);
                return;
            }

            var hashedPassword = StringCryptography.GetSaltedHashedValueAsString(password, user.Salt);

            if (hashedPassword != user.Password)
            {
                using var packetError = new NetworkPacket();
                LoginReply.Write(packetError, "", "InvalidUsernamePassword", "");
                packetError.Send(peer);
                return;
            }

            var loggedInPlayer = PlayerManager.GetPlayer(username);

            if (loggedInPlayer != null && (loggedInPlayer.IsLoggedIn || loggedInPlayer.IsPlaying))
            {
                using var packetError = new NetworkPacket();
                LoginReply.Write(packetError, "", "AlreadyLoggedIn", "");
                packetError.Send(peer);
                return;
            }

            var player = PlayerManager.GetPlayer(peer);

            var authToken = Guid.NewGuid().ToString();
            user.AuthToken = authToken;

            player.User = user;
            player.IsLoggedIn = true;

            using var packet = new NetworkPacket();
            LoginReply.Write(packet, authToken, "", GameServer.WorldSeed);
            packet.Send(peer);

            Logging.Information("User logged in: {username}.", username);
        }
    } // NetworkServer
}
