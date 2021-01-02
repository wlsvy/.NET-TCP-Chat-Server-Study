using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Server.Core
{
    public sealed class ClientConnectionManager : IDisposable
    {
        private readonly AsyncTCPAcceptor m_Acceptor;
        private readonly SessionManager m_SessionManager;

        public ClientConnectionManager(SessionManager sessionManager)
        {
            m_Acceptor = new AsyncTCPAcceptor(OnNewConnection);
            m_SessionManager = sessionManager;
        }

        public void Dispose()
        {
            m_Acceptor?.Dispose();
        }

        private void OnNewConnection(Socket socket)
        {

        }

    }
}
