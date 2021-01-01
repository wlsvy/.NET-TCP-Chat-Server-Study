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

        public static void TryConnect(IPAddress ip, int port, Queue<TimeSpan> leftTimeoutList, Action<bool, Socket, object> onCompleted)
        {
            if(ip == null)
            {
                throw new ArgumentNullException(nameof(ip));
            }

            var remoteEndPoint = new IPEndPoint(ip, port);

            DoTryConnect(remoteEndPoint, leftTimeoutList, onCompleted);
        }

        private static void DoTryConnect(EndPoint remoteEndPoint, Queue<TimeSpan> leftTimeoutList, Action<bool, Socket, object> onCompleted)
        {
            if(remoteEndPoint == null)
            {
                throw new ArgumentNullException(nameof(remoteEndPoint));
            }

            var socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            var args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = remoteEndPoint;
            args.Completed += OnConnect;

            try
            {
                bool willRaiseEventLater = socket.ConnectAsync(args);
                if (!willRaiseEventLater)
                {
                    //연결이 바로 완료될 시
                    ProcessConnect(args);
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"서버 연결 실패 - [{e.Message}]");
                DisposeConnectionArgs(args);
            }
        }

        private static void OnConnect(object sender, SocketAsyncEventArgs args)
        {
            ProcessConnect(args);
        }

        private static void ProcessConnect(SocketAsyncEventArgs args)
        {

        }

        private static void DisposeConnectionArgs(SocketAsyncEventArgs args)
        {
            args.Dispose();
        }

        public void Connect()
        {

        }
    }
}
