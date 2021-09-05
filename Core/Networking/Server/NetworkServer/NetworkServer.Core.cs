using ElementEngine;
using ElementEngine.ECS;
using FinalFrontier.Components;
using FinalFrontier.Database.Tables;
using FinalFrontier.GameData;
using FinalFrontier.Networking;
using FinalFrontier.Networking.Packets;
using FinalFrontier.Networking.Server;
using FinalFrontier.Utility;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace FinalFrontier.Networking
{
    public partial class NetworkServer : IDisposable
    {
        public static StringCryptography StringCryptography = new StringCryptography();

        public const int TicksPerSecond = 30;
        public const float SecondsPerTick = 1f / TicksPerSecond;

        public readonly GameServer GameServer;

        public NetManager NetManager;
        public EventBasedNetListener Listener;
        public NetworkPacket NextPacket;
        public float CurrentTickTime = 0f;
        public double WorldTime = 10000000;

        public Database Database;
        public PlayerManager PlayerManager;

        public void Dispose()
        {
            Database?.Dispose();
            Database = null;
        }

        public NetworkServer(GameServer gameServer)
        {
            GameServer = gameServer;
            PlayerManager = new PlayerManager(this);
        }

        public void Load()
        {
            Database = new Database();
            Database.Load();

            NextPacket = new NetworkPacket();

            Listener = new EventBasedNetListener();
            NetManager = new NetManager(Listener);

            Listener.ConnectionRequestEvent += OnRequest;
            Listener.PeerConnectedEvent += OnPeerConnected;
            Listener.PeerDisconnectedEvent += OnPeerDisconnected;
            Listener.NetworkReceiveEvent += OnNetworkReceive;
            Listener.NetworkErrorEvent += OnNetworkError;

            Logging.Information("Starting server on port {port}.", Globals.ServerPort);
            NetManager.Start(Globals.ServerPort);
            Logging.Information("Server started on port {port}.", Globals.ServerPort);
        }

        private void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            var address = ServerUtility.GetIPAddress(endPoint);
            Logging.Error("Network error [{address}] {error}.", address, socketError.ToString());
        }

        private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var dataCount = reader.GetInt();
            if (dataCount <= 0)
                return;

            var data = reader.GetBytesWithLength();
            HandlePacket(dataCount, data, peer);
        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Logging.Information("Client disconnected: {id}, {ip}, {port}", peer.Id, peer.EndPoint.Address.ToString(), peer.EndPoint.Port.ToString());

            var player = PlayerManager.GetPlayer(peer);

            if (player != null && player.Ship.IsAlive)
                GameServer.ServerWorldManager.DestroyEntity(NextPacket, player.Ship);

            PlayerManager.RemovePlayer(peer);
        }

        private void OnPeerConnected(NetPeer peer)
        {
            Logging.Information("Client connected: {id}, {ip}, {port}", peer.Id, peer.EndPoint.Address.ToString(), peer.EndPoint.Port.ToString());
            PlayerManager.AddPlayer(peer);
        }

        private void OnRequest(ConnectionRequest request)
        {
            var address = ServerUtility.GetIPAddress(request);
            Logging.Information("Connection request {address}.", address);

            request.AcceptIfKey(Globals.ConnectionKey);
        }

        public void Update(GameTimer gameTimer)
        {
            WorldTime += gameTimer.DeltaS;
            NetManager.PollEvents();

            CurrentTickTime += gameTimer.DeltaS;

            while (CurrentTickTime > SecondsPerTick)
            {
                foreach (var player in PlayerManager.Players)
                {
                    if (!player.IsPlaying)
                        continue;

                    if (!player.Ship.IsAlive)
                        GameServer.ServerWorldManager.SpawnPlayerShip(GameServer, Database, player);
                }

                WorldUpdateReply.Write(NextPacket, WorldTime);

                foreach (var loop in NetworkSyncManager.ServerEveryFrameSyncLoops)
                    loop(NextPacket);

                foreach (var loop in NetworkSyncManager.ServerTempSyncLoops)
                    loop(NextPacket);

                if (NextPacket.DataCount > 0)
                {
                    SendPacketToPlaying(NextPacket);
                    NextPacket?.Dispose();
                    NextPacket = new NetworkPacket();
                }

                CurrentTickTime -= SecondsPerTick;
            }
        } // Update

        public void SendPacketToPlaying(NetworkPacket packet)
        {
            foreach (var player in PlayerManager.Players)
            {
                if (!player.IsPlaying)
                    continue;

                packet.Send(player.Peer);
            }
        }

        public void HandlePacket(int dataCount, byte[] data, NetPeer peer)
        {
            using var ms = new MemoryStream(NetworkPacket.Decompress(data));
            using var reader = new BinaryReader(ms);

            for (var i = 0; i < dataCount; i++)
            {
                var type = (NetworkPacketDataType)reader.ReadInt32();
                HandlePacketData(type, reader, peer);
            }
        }

        public void HandlePacketData(NetworkPacketDataType type, BinaryReader reader, NetPeer peer)
        {
            switch (type)
            {
                case NetworkPacketDataType.ServerStatus:
                    {
                        using var packet = new NetworkPacket();
                        ServerStatusReply.Write(packet, PlayerManager.Players.Count);
                        packet.Send(peer);
                    }
                    break;

                case NetworkPacketDataType.Register:
                    {
                        HandleRegister(reader, peer);
                    }
                    break;

                case NetworkPacketDataType.Login:
                    {
                        HandleLogin(reader, peer);
                    }
                    break;

                case NetworkPacketDataType.JoinGame:
                    {
                        var auth = CheckAuth(reader, out var username, out var authToken, out var player);

                        if (!auth)
                        {
                            LogAuthFailed(type, username, authToken);
                            return;
                        }

                        player.IsPlaying = true;

                        using var packet = new NetworkPacket();

                        foreach (var loop in NetworkSyncManager.ServerPlayerJoinedSyncLoops)
                            loop(packet);

                        packet.Send(peer);

                        GameServer.ServerWorldManager.SpawnPlayerShip(GameServer, Database, player);
                        Logging.Information("Player joined game: {username}.", player.User.Username);
                    }
                    break;

                case NetworkPacketDataType.PlayerMoveToPosition:
                    {
                        var auth = CheckAuth(reader, out var username, out var authToken, out var player);

                        if (!auth)
                        {
                            LogAuthFailed(type, username, authToken);
                            return;
                        }

                        PlayerMoveToPositionRequest.Read(reader, out var position, out var sectorPosition);

                        EntityUtility.ImmediateRemoveMovementComponents(player.Ship);
                        player.Ship.TryAddComponent(new MoveToPosition()
                        {
                            Position = position,
                            SectorPosition = sectorPosition,
                            Orbit = false,
                        });
                    }
                    break;

                case NetworkPacketDataType.Logout:
                    {
                        var player = PlayerManager.GetPlayer(peer);

                        if (player != null && player.Ship.IsAlive)
                            GameServer.ServerWorldManager.DestroyEntity(NextPacket, player.Ship);

                        PlayerManager.RemovePlayer(peer);
                    }
                    break;

                case NetworkPacketDataType.ChatMessage:
                    {
                        var auth = CheckAuth(reader, out var username, out var authToken, out var player);

                        if (!auth)
                        {
                            LogAuthFailed(type, username, authToken);
                            return;
                        }

                        ChatMessageRequest.Read(reader, out var message);
                        var replyMessage = player.User.Username + ": " + message;

                        using var packet = new NetworkPacket();
                        ChatMessageReply.Write(packet, replyMessage);
                        SendPacketToPlaying(packet);
                    }
                    break;

                case NetworkPacketDataType.EquipComponent:
                    {
                        var auth = CheckAuth(reader, out var username, out var authToken, out var player);

                        if (!auth)
                        {
                            LogAuthFailed(type, username, authToken);
                            return;
                        }

                        EquipComponentRequest.Read(reader, out var componentType, out var seed);

                        ref var inventory = ref player.Ship.GetComponent<Inventory>();
                        ref var ship = ref player.Ship.GetComponent<Ship>();

                        var itemIndex = inventory.Items.FindIndex((item) => item.Seed == seed);

                        if (itemIndex == -1)
                            return;

                        var shipData = GameDataManager.Ships[ship.ShipType];
                        var replacing = ship.ShipComponentData[componentType];

                        using var command = Database.Connection.CreateCommand();
                        var item = inventory.Items[itemIndex];
                        item.Remove(command);
                        inventory.Items.RemoveAt(itemIndex);

                        var newItem = new InventoryItem()
                        {
                            Username = item.Username,
                            ComponentType = componentType,
                            Seed = replacing.Seed,
                            Quality = replacing.Quality,
                            ClassType = null,
                        };

                        newItem.Insert(command);
                        inventory.Items.Add(newItem);

                        var componentSlotData =  new ShipComponentSlotData()
                        {
                            Slot = componentType,
                            Seed = item.Seed,
                            Quality = item.Quality,
                        };

                        if (componentType == ShipComponentType.Engine)
                            componentSlotData.ComponentData = new ShipEngineData(componentSlotData);

                        ship.ShipComponentData[componentType] = componentSlotData;

                        switch (componentType)
                        {
                            case ShipComponentType.Shield:
                                {
                                    var shield = new Shield(shipData, ref ship);
                                    player.Ship.TryAddComponent(shield);
                                    EntityUtility.SetNeedsTempNetworkSync<Shield>(player.Ship);
                                }
                                break;

                            case ShipComponentType.Armour:
                                {
                                    var armour = new Armour(shipData, ref ship);
                                    player.Ship.TryAddComponent(armour);
                                    EntityUtility.SetNeedsTempNetworkSync<Armour>(player.Ship);
                                }
                                break;
                        }

                        EntityUtility.SetNeedsTempNetworkSync<Ship>(player.Ship);
                        EntityUtility.SetNeedsTempNetworkSync<Inventory>(player.Ship);
                    }
                    break;

                case NetworkPacketDataType.EquipWeapon:
                    {
                        var auth = CheckAuth(reader, out var username, out var authToken, out var player);

                        if (!auth)
                        {
                            LogAuthFailed(type, username, authToken);
                            return;
                        }

                        EquipWeaponRequest.Read(reader, out var slot, out var seed);

                        ref var inventory = ref player.Ship.GetComponent<Inventory>();
                        ref var ship = ref player.Ship.GetComponent<Ship>();
                        ref var drawable = ref player.Ship.GetComponent<Drawable>();

                        var itemIndex = inventory.Items.FindIndex((item) => item.Seed == seed);

                        if (itemIndex == -1)
                            return;

                        var shipData = GameDataManager.Ships[ship.ShipType];
                        var replacing = ship.ShipWeaponData[slot];

                        using var command = Database.Connection.CreateCommand();
                        var item = inventory.Items[itemIndex];
                        item.Remove(command);
                        inventory.Items.RemoveAt(itemIndex);

                        var newItem = new InventoryItem()
                        {
                            Username = item.Username,
                            ComponentType = null,
                            Seed = replacing.Seed,
                            Quality = replacing.Quality,
                            ClassType = shipData.Turrets[slot].Class,
                        };

                        newItem.Insert(command);
                        inventory.Items.Add(newItem);

                        ship.ShipWeaponData[slot] = new ShipWeaponSlotData()
                        {
                            Slot = slot,
                            Seed = item.Seed,
                            Quality = item.Quality,
                        };

                        GameServer.ServerWorldManager.DestroyEntity(NextPacket, ship.Turrets[slot]);
                        ship.Turrets[slot] = TurretPrefabs.ShipTurret(GameServer, player.Ship, drawable.Layer + 1, ship.ShipWeaponData[slot], shipData);

                        EntityUtility.SetNeedsTempNetworkSync<Ship>(player.Ship);
                        EntityUtility.SetNeedsTempNetworkSync<Inventory>(player.Ship);
                    }
                    break;
            }
        } // HandlePacketData

        public void SendSystemMessage(Player player, string message)
        {
            using var packet = new NetworkPacket();
            ChatMessageReply.Write(packet, "System: " + message);
            packet.Send(player.Peer);
        }

        public bool CheckAuth(BinaryReader reader, out string username, out string authToken, out Player player)
        {
            PacketUtil.ReadAuth(reader, out username, out authToken);
            
            player = PlayerManager.GetPlayer(username);

            if (player == null)
                return false;
            if (player.User.AuthToken != authToken)
                return false;

            return true;
        }

        public void LogAuthFailed(NetworkPacketDataType dataType, string username, string authToken)
        {
            Logging.Error("Auth failed on {type} {username} {authToken}.", dataType, username, authToken);
        }

    } // NetworkServer
}
