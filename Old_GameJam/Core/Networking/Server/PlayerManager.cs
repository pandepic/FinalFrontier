using ElementEngine.ECS;
using FinalFrontier.Components;
using FinalFrontier.Database.Tables;
using FinalFrontier.Networking.Packets;
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
        public int RespawnTicks;
        public UserShip UserShip;
    }

    public class PlayerManager
    {
        public NetworkServer NetworkServer;
        public List<Player> Players = new List<Player>();

        public PlayerManager(NetworkServer networkServer)
        {
            NetworkServer = networkServer;
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
            var index = Players.FindIndex((p) => p.User != null && p.User.Username == username);
            return index == -1 ? null : Players[index];
        }

        public void GiveExpMoney(string username, int exp, int money)
        {
            var player = GetPlayer(username);

            if (player == null || !player.IsPlaying || !player.Ship.IsAlive)
                return;

            ref var playerShip = ref player.Ship.GetComponent<PlayerShip>();

            playerShip.Money += money;
            playerShip.Exp += (int)(exp * 1.25f);
            playerShip.CheckRankUp();
            EntityUtility.SetNeedsTempNetworkSync<PlayerShip>(player.Ship);

            player.User.Money = (uint)playerShip.Money;
            player.User.Exp = (uint)playerShip.Exp;
            player.User.Rank = playerShip.Rank;

            using var command = NetworkServer.Database.Connection.CreateCommand();
            player.User.Update(command);

            NetworkServer.SendSystemMessage(player, $"Gained {exp} exp and {money} credits.");

        } // GiveExpMoney

        public void GiveMoney(string username, int money)
        {
            var player = GetPlayer(username);

            if (player == null || !player.IsPlaying || !player.Ship.IsAlive)
                return;

            ref var playerShip = ref player.Ship.GetComponent<PlayerShip>();

            playerShip.Money += money;
            EntityUtility.SetNeedsTempNetworkSync<PlayerShip>(player.Ship);

            player.User.Money = (uint)playerShip.Money;
            using var command = NetworkServer.Database.Connection.CreateCommand();
            player.User.Update(command);

            NetworkServer.SendSystemMessage(player, $"Gained {money} credits.");
        }

        public void SpendMoney(string username, int money)
        {
            var player = GetPlayer(username);

            if (player == null || !player.IsPlaying || !player.Ship.IsAlive)
                return;

            ref var playerShip = ref player.Ship.GetComponent<PlayerShip>();

            playerShip.Money -= money;
            EntityUtility.SetNeedsTempNetworkSync<PlayerShip>(player.Ship);

            player.User.Money = (uint)playerShip.Money;
            using var command = NetworkServer.Database.Connection.CreateCommand();
            player.User.Update(command);

            NetworkServer.SendSystemMessage(player, $"Spent {money} credits.");
        }
    } // PlayerManager
}
