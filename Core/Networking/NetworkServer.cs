using ElementEngine;
using FinalFrontier.Networking;
using FinalFrontier.Networking.Packets;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace FinalFrontier.Networking
{
    public class PeerInfo
    {
        public int ID;
    }

    public class NetworkServer
    {
        public const int TicksPerSecond = 60;
        public const float SecondsPerTick = 1f / TicksPerSecond;

        public NetManager NetManager;
        public EventBasedNetListener Listener;
        public NetworkPacket NextPacket;
        public float CurrentTickTime = 0f;

        public Database Database;

        public Dictionary<int, PeerInfo> ConnectedPeers = new Dictionary<int, PeerInfo>();

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
            var address = ServerUtil.GetIPAddress(endPoint);
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
        }

        private void OnPeerConnected(NetPeer peer)
        {
            Logging.Information("New connection: {id}, {ip}, {port}", peer.Id, peer.EndPoint.Address.ToString(), peer.EndPoint.Port.ToString());
        }

        private void OnRequest(ConnectionRequest request)
        {
            var address = ServerUtil.GetIPAddress(request);
            Logging.Information("Connection request {address}.", address);

            request.AcceptIfKey(Globals.ConnectionKey);
        }

        public void Update(GameTimer gameTimer)
        {
            NetManager.PollEvents();

            CurrentTickTime += gameTimer.DeltaS;

            while (CurrentTickTime > SecondsPerTick)
            {
                // todo: add packet data

                if (NextPacket.DataCount > 0)
                {
                    NextPacket.SendAll(NetManager);
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
                        var request = new ServerStatusRequest();
                        var reply = new ServerStatusReply(ConnectedPeers.Count);

                        var packet = new NetworkPacket();
                        reply.Write(packet);
                        packet.Send(peer);
                    }
                    break;
            }
        } // HandlePacketData

    } // NetworkServer
}
