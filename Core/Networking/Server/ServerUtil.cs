using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking
{
    public static class ServerUtil
    {
        public static string GetIPAddress(NetPeer peer)
        {
            if (peer == null)
                return "";

            return GetIPAddress(peer.EndPoint);
        }

        public static string GetIPAddress(ConnectionRequest request)
        {
            if (request == null)
                return "";

            return GetIPAddress(request.RemoteEndPoint);
        }

        public static string GetIPAddress(IPEndPoint endPoint)
        {
            var address = "";

            if (endPoint != null && endPoint.Address != null)
                address = endPoint.Address.ToString();

            return address;
        }

    } // ServerUtil
}
