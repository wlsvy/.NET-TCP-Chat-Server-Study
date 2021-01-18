using System;
using System.Net;
using System.Net.Sockets;
using Shared.Logger;
using Shared.Interface;
using System.Diagnostics;

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
            m_Socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            m_OnNewConnection = onNewConnection ?? throw new ArgumentNullException(nameof(onNewConnection));
        }

        public void Bind(IPAddress ip, int port)
        {
            _ = ip ?? throw new ArgumentNullException(nameof(ip));

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

            Log.I.Info($"Listen 시작. Address : {LocalEndpoint.ToString()}");
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
                Log.I.Warn($"{nameof(AsyncTcpAcceptor)}.{nameof(this.ProcessAccept)} Accept 실패");
                return;
            }
            Log.I.Info($"{nameof(AsyncTcpAcceptor)}.{nameof(ProcessAccept)} Accept 성공");

            m_OnNewConnection(args.AcceptSocket);
            args.AcceptSocket = null;
            StartAccept(args);
        }

        private void StartAccept(SocketAsyncEventArgs args)
        {
            Debug.Assert(args.AcceptSocket == null);

            Log.I.Info($"{nameof(AsyncTcpAcceptor)}.{nameof(StartAccept)}!!");

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
                Log.I.Error($"Accept 실패 [{e.Message}]", e);
            }
        }
    }
}
