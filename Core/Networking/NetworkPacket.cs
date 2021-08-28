using ElementEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;
using System.IO;

namespace FinalFrontier.Networking
{
    public class NetworkPacket
    {
        protected static NetDataWriter _netDataWriter = new NetDataWriter();

        public int DataCount = 0;
        public MemoryStream MemoryStream { get; set; }
        public BinaryWriter Writer { get; set; }

        public NetworkPacket()
        {
            MemoryStream = new MemoryStream();
            Writer = new BinaryWriter(MemoryStream);
        }

        public void Dispose()
        {
            MemoryStream?.Dispose();
            MemoryStream = null;
            Writer?.Dispose();
            Writer = null;
        }

        public static byte[] Decompress(byte[] compressed)
        {
            return Compression.Unzip(compressed);
        }

        public void SendAll(NetManager netManager)
        {
            if (DataCount <= 0)
                return;
            if (netManager == null)
                return;

            SendAll(netManager.ConnectedPeerList);
        }

        public void SendAll(List<NetPeer> peers)
        {
            if (DataCount <= 0)
                return;

            var data = GetCompressedData();

            if (peers == null || peers.Count == 0)
                return;

            _netDataWriter.Reset();
            _netDataWriter.Put(DataCount);
            _netDataWriter.PutBytesWithLength(data);

            foreach (var peer in peers)
            {
                peer.Send(_netDataWriter, DeliveryMethod.ReliableOrdered);
            }
        }

        public void Send(NetPeer peer)
        {
            if (DataCount <= 0)
                return;

            var data = GetCompressedData();

            _netDataWriter.Reset();
            _netDataWriter.Put(DataCount);
            _netDataWriter.PutBytesWithLength(data);

            peer.Send(_netDataWriter, DeliveryMethod.ReliableOrdered);
        }

        public byte[] GetCompressedData()
        {
            MemoryStream.Position = 0;
            return Compression.Zip(MemoryStream);
        }

    } // NetworkPacket
}
