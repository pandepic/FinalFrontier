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
        public void HandleRegister(BinaryReader reader, NetPeer peer)
        {
            RegisterRequest.Read(reader, out var username, out var password);

            if (Database.Users.ContainsKey(username))
            {
                using var packet = new NetworkPacket();
                RegisterReply.Write(packet, "UsernameTaken");
                packet.Send(peer);
                return;
            }

            var salt = Guid.NewGuid().ToString();
            var hashedPassword = StringCryptography.GetSaltedHashedValueAsString(password, salt);

            var user = new User()
            {
                Username = username,
                Password = hashedPassword,
                Salt = salt,
                Money = 0,
                AuthToken = "",
                Registered = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
            };

            Database.InsertUser(user);

            Logging.Information("User registered: {username}.", username);
        }
    } // NetworkServer
}
