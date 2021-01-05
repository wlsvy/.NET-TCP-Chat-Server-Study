using System;
using System.Net;
using System.Net.Sockets;
using Shared.Logger;

namespace Shared.Network
{
    public class AsyncTcpAcceptor : IDisposable
    {
        private readonly Socket m_Socket;
        public EndPoint LocalEndpoint => m_Socket.LocalEndPoint;
        private readonly Action<Socket> m_OnNewConnection;

        private bool m_IsDisposed = false;

        public AsyncTcpAcceptor(Action<Socket> onNewConnection)
        {
            if(onNewConnection == null)
            {
                throw new ArgumentNullException(nameof(onNewConnection));
            }
            m_Socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            m_OnNewConnection = onNewConnection;
        }

        public void Bind(IPAddress ip, int port)
        {
            if(ip == null)
            {
                throw new ArgumentNullException(nameof(ip));
            }

            var localEndPoint = new IPEndPoint(ip, port);
            m_Socket.Bind(localEndPoint);
        }

        public void ListenAndStart(int listenBacklog)
        {
            m_Socket.Listen(listenBacklog);

            for(int i = 0; i < listenBacklog; i++)
            {
                var args = new SocketAsyncEventArgs();
                args.AcceptSocket = null;
                args.Completed += OnAcceptCompleted;
            }
        }

        public void Dispose()
        {
            if (m_IsDisposed)
            {
                return;
            }
            m_IsDisposed = true;

            m_Socket.Close();
            m_Socket.Dispose();
        }

        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            ProcessAccept(args);
        }

        private void ProcessAccept(SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
            {
                Log.I.Error($"Accept 실패");
                return;
            }

            m_OnNewConnection(args.AcceptSocket);
            args.AcceptSocket = null;
            StartAccept(args);
        }

        private void StartAccept(SocketAsyncEventArgs args)
        {
            if(args.AcceptSocket != null)
            {
                throw new ArgumentNullException(nameof(args.AcceptSocket));
            }

            try
            {
                bool willRaiseEventLater = m_Socket.AcceptAsync(args);
                if (!willRaiseEventLater)
                {
                    ProcessAccept(args);
                }
            }
            catch(Exception e)
            {
                Log.I.Error($"Accept 실패 [{e.Message}]");
            }
        }
    }
}
