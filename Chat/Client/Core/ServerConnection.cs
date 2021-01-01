using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Client.Core
{
    public sealed class ServerConnection
    {
        public readonly long ConnectionId;

        public readonly IPAddress ServerIp;
        public readonly EndPoint RemoteEndPoint;
        public readonly Socket ConnectSocket;

        public ServerConnection(long id, IPAddress serverIp, EndPoint remoteEndPoint, Socket socket)
        {
            ConnectionId = id;
            ServerIp = serverIp;
            RemoteEndPoint = remoteEndPoint;
            ConnectSocket = socket;
        }

        

        public void Connect()
        {

        }
    }
}
