using System;
using System.Net;
using System.Net.Sockets;
using Shared.Logger;
using Shared.Interface;

namespace Shared.Network
{
    public class AsyncTcpAcceptor : IDisposable
    {
        private readonly Socket m_Socket;
        public EndPoint LocalEndpoint => m_Socket.LocalEndPoint;
        private readonly Action<Socket> m_OnNewConnection;
        private readonly ILogger m_Logger;

        private bool m_IsDisposed = false;

        public AsyncTcpAcceptor(Action<Socket> onNewConnection, ILogger logger)
        {
            if(onNewConnection == null)
            {
                throw new ArgumentNullException(nameof(onNewConnection));
            }
            m_Socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            m_Logger = logger;
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
                StartAccept(args);
            }

            m_Logger?.Info($"Listen 시작. Address : {LocalEndpoint.ToString()}");
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
                m_Logger?.Warn($"{nameof(AsyncTcpAcceptor)}.{nameof(this.ProcessAccept)} Accept 실패");
                return;
            }
            m_Logger?.Info($"{nameof(AsyncTcpAcceptor)}.{nameof(ProcessAccept)}!!");

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
            m_Logger?.Info($"{nameof(AsyncTcpAcceptor)}.{nameof(StartAccept)}!!");

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
                m_Logger?.Error($"Accept 실패 [{e.Message}]", e);
            }
        }
    }
}
