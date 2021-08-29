using ElementEngine;
using ElementEngine.Timer;
using FinalFrontier.Networking.Packets;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking
{
    public class NetworkClient
    {
        public NetManager NetManager;
        public EventBasedNetListener Listener;

        public int ServerPlayers = 0;

        public NetPeer Server => NetManager.FirstPeer;
        public bool IsConnected => NetManager.FirstPeer != null && NetManager.FirstPeer.ConnectionState == ConnectionState.Connected;

        public NetworkClient()
        {
            Listener = new EventBasedNetListener();
            NetManager = new NetManager(Listener);

            Listener.ConnectionRequestEvent += OnRequest;
            Listener.PeerConnectedEvent += OnPeerConnected;
            Listener.PeerDisconnectedEvent += OnPeerDisconnected;
            Listener.NetworkReceiveEvent += OnNetworkReceive;
            Listener.NetworkErrorEvent += OnNetworkError;

            NetManager.Start();
            NetManager.Connect(Globals.ServerAddress, Globals.ServerPort, Globals.ConnectionKey);
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
            // keep trying to reconnect
            Logging.Information("Connection to server lost, trying to re-connect.");
            NetManager.Connect(Globals.ServerAddress, Globals.ServerPort, Globals.ConnectionKey);
        }

        private void OnPeerConnected(NetPeer peer)
        {
            Logging.Information("Connected to server.");
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
                        var reply = new ServerStatusReply(reader);
                        ServerPlayers = reply.Players;
                    }
                    break;
            }
        } // HandlePacketData

    } // NetworkClient
}
