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
                    {
                        if (player.RespawnTicks < 0)
                        {
                            player.RespawnTicks = 10;
                            continue;
                        }

                        player.RespawnTicks -= 1;

                        if (player.RespawnTicks == 0)
                        {
                            GameServer.ServerWorldManager.SpawnPlayerShip(GameServer, Database, player);
                            player.RespawnTicks = -1;
                        }
                    }
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
                                    ref var prevShield = ref player.Ship.GetComponent<Shield>();
                                    var shield = new Shield(shipData, ref ship);
                                    shield.CurrentValue = Math.Min(shield.BaseValue, prevShield.CurrentValue);
                                    player.Ship.TryAddComponent(shield);
                                    EntityUtility.SetNeedsTempNetworkSync<Shield>(player.Ship);
                                }
                                break;

                            case ShipComponentType.Armour:
                                {
                                    ref var prevArmour = ref player.Ship.GetComponent<Armour>();
                                    var armour = new Armour(shipData, ref ship);
                                    armour.CurrentValue = Math.Min(armour.BaseValue, prevArmour.CurrentValue);
                                    player.Ship.TryAddComponent(armour);
                                    EntityUtility.SetNeedsTempNetworkSync<Armour>(player.Ship);
                                }
                                break;
                        }

                        foreach (var component in player.UserShip.Components)
                        {
                            if (component.Slot == componentType)
                            {
                                component.Seed = item.Seed;
                                component.Quality = item.Quality;
                            }
                        }

                        player.UserShip.Update(command);

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

                        foreach (var weapon in player.UserShip.Weapons)
                        {
                            if (weapon.Slot == slot)
                            {
                                weapon.Seed = item.Seed;
                                weapon.Quality = item.Quality;
                            }
                        }

                        player.UserShip.Update(command);

                        EntityUtility.SetNeedsTempNetworkSync<Ship>(player.Ship);
                        EntityUtility.SetNeedsTempNetworkSync<Inventory>(player.Ship);
                    }
                    break;

                case NetworkPacketDataType.SellItem:
                    {
                        var auth = CheckAuth(reader, out var username, out var authToken, out var player);

                        if (!auth)
                        {
                            LogAuthFailed(type, username, authToken);
                            return;
                        }

                        SellItemRequest.Read(reader, out var seed);

                        ref var inventory = ref player.Ship.GetComponent<Inventory>();

                        var itemIndex = inventory.Items.FindIndex((item) => item.Seed == seed);

                        if (itemIndex == -1)
                            return;

                        var item = inventory.Items[itemIndex];
                        var price = item.Quality switch
                        {
                            QualityType.Common => 250,
                            QualityType.Uncommon => 500,
                            QualityType.Rare => 2000,
                            QualityType.Legendary => 10000,
                            _ => throw new NotImplementedException(),
                        };

                        using var command = Database.Connection.CreateCommand();
                        item.Remove(command);
                        inventory.Items.RemoveAt(itemIndex);

                        PlayerManager.GiveMoney(username, price);
                        EntityUtility.SetNeedsTempNetworkSync<Inventory>(player.Ship);
                    }
                    break;

                case NetworkPacketDataType.BuyShip:
                    {
                        var auth = CheckAuth(reader, out var username, out var authToken, out var player);

                        if (!auth)
                        {
                            LogAuthFailed(type, username, authToken);
                            return;
                        }

                        BuyShipRequest.Read(reader, out var shipName);

                        ref var transform = ref player.Ship.GetComponent<Transform>();
                        if (!GameServer.ServerWorldManager.ColonisedSectors.Contains(transform.TransformedSectorPosition))
                        {
                            SendSystemMessage(player, "You can only buy ships in a friendly sector.");
                            return;
                        }

                        var buyShipData = GameDataManager.Ships[shipName];

                        if ((int)buyShipData.RequiredRank > (int)player.User.Rank)
                        {
                            SendSystemMessage(player, "Your rank is too low to buy this ship.");
                            return;
                        }

                        if (buyShipData.Cost > player.User.Money)
                        {
                            SendSystemMessage(player, "Not enough credits to buy this ship.");
                            return;
                        }

                        PlayerManager.SpendMoney(username, buyShipData.Cost);

                        using var command = Database.Connection.CreateCommand();
                        
                        ref var ship = ref player.Ship.GetComponent<Ship>();
                        var prevShipData = GameDataManager.Ships[ship.ShipType];

                        foreach (var (slot, component) in ship.ShipComponentData)
                        {
                            if (component.Quality == QualityType.Common)
                                continue;

                            var newItem = new InventoryItem()
                            {
                                Username = username,
                                ComponentType = slot,
                                Seed = component.Seed,
                                Quality = component.Quality,
                                ClassType = null,
                            };

                            newItem.Insert(command);
                        }

                        foreach (var (slot, weapon) in ship.ShipWeaponData)
                        {
                            if (weapon.Quality == QualityType.Common)
                                continue;

                            var newItem = new InventoryItem()
                            {
                                Username = username,
                                ComponentType = null,
                                Seed = weapon.Seed,
                                Quality = weapon.Quality,
                                ClassType = prevShipData.Turrets[slot].Class,
                            };

                            newItem.Insert(command);
                        }

                        foreach (var (_, turret) in ship.Turrets)
                            GameServer.ServerWorldManager.DestroyEntity(NextPacket, turret);

                        GameServer.ServerWorldManager.DestroyEntity(NextPacket, player.Ship);

                        UserShip.ClearShips(command, username);

                        var newShip = new UserShip()
                        {
                            Username = player.User.Username,
                            ShipName = buyShipData.Name,
                            IsActive = true,
                            Weapons = new List<UserShipWeapon>(),
                            Components = new List<UserShipComponent>(),
                        };

                        foreach (var slot in Enum.GetValues<ShipComponentType>())
                        {
                            newShip.Components.Add(new UserShipComponent()
                            {
                                Username = player.User.Username,
                                ShipName = buyShipData.Name,
                                Slot = slot,
                                Seed = Guid.NewGuid().ToString(),
                                Quality = QualityType.Common,
                            });
                        }

                        for (int i = 0; i < buyShipData.Turrets.Count; i++)
                        {
                            newShip.Weapons.Add(new UserShipWeapon()
                            {
                                Username = player.User.Username,
                                ShipName = buyShipData.Name,
                                Slot = i,
                                Seed = Guid.NewGuid().ToString(),
                                Quality = QualityType.Common,
                            });
                        }

                        newShip.Insert(command);
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
