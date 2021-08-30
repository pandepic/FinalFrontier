using ElementEngine.ECS;
using FinalFrontier.Database.Tables;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking.Server
{
    public class Player
    {
        public NetPeer Peer;
        public User User;
        public Entity Ship;
        public bool IsLoggedIn;
        public bool IsPlaying;
    }

    public class PlayerManager
    {
        public List<Player> Players = new List<Player>();

        public PlayerManager()
        {
        }

        public void AddPlayer(NetPeer peer)
        {
            Players.Add(new Player()
            {
                Peer = peer,
            });
        }

        public void RemovePlayer(NetPeer peer)
        {
            var player = GetPlayer(peer);

            if (player != null)
                Players.Remove(player);
        }

        public Player GetPlayer(NetPeer peer)
        {
            var index = Players.FindIndex((p) => p.Peer.Id == peer.Id);
            return index == -1 ? null : Players[index];
        }

        public Player GetPlayer(string username)
        {
            var index = Players.FindIndex((p) => p.User.Username == username);
            return index == -1 ? null : Players[index];
        }

    } // PlayerManager
}
