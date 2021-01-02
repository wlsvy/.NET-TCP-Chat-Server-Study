using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server.Core
{
    public sealed class ClientConnectionManager : IDisposable
    {
        private readonly AsyncTCPAcceptor m_Acceptor;
        private readonly SessionManager m_SessionManager;
        private readonly ConcurrentDictionary<long, ClientConnection> m_Connections;

        private long m_PreviousClientConnectionId = -1;
        private bool m_IsDisposed = false;

        public ClientConnectionManager(SessionManager sessionManager)
        {
            m_Acceptor = new AsyncTCPAcceptor(OnNewConnection);
            m_SessionManager = sessionManager;
            m_Connections = new ConcurrentDictionary<long, ClientConnection>();
        }

        public void Dispose()
        {
            if (m_IsDisposed)
            {
                return;
            }
            m_IsDisposed = true;

            m_Acceptor?.Dispose();
        }

        private void OnNewConnection(Socket socket)
        {
            long nextConnectionId = GenerateClientConnectionId();

        }

        private long GenerateClientConnectionId()
        {
            return Interlocked.Increment(ref m_PreviousClientConnectionId);
        }

    }
}
