﻿using ElementEngine;
using ElementEngine.ECS;
using FinalFrontier.Components;
using FinalFrontier.Database.Tables;
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

        public const int TicksPerSecond = 60;
        public const float SecondsPerTick = 1f / TicksPerSecond;

        public readonly GameServer GameServer;

        public NetManager NetManager;
        public EventBasedNetListener Listener;
        public NetworkPacket NextPacket;
        public float CurrentTickTime = 0f;
        public double WorldTime = 10000000;

        public Database Database;
        public PlayerManager PlayerManager = new PlayerManager();

        public void Dispose()
        {
            Database?.Dispose();
            Database = null;
        }

        public NetworkServer(GameServer gameServer)
        {
            GameServer = gameServer;
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
                    foreach (var player in PlayerManager.Players)
                    {
                        if (!player.IsPlaying)
                            continue;

                        NextPacket.Send(player.Peer);
                    }

                    NextPacket?.Dispose();
                    NextPacket = new NetworkPacket();
                }

                CurrentTickTime -= SecondsPerTick;
            }
        } // Update

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

                        var packet = new NetworkPacket();

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
            }
        } // HandlePacketData

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