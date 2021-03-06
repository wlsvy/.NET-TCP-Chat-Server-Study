﻿using Shared.Network;
using Shared.Util;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server.Network
{
    public sealed class ClientConnectionManager : IDisposable
    {
        private readonly AsyncTcpAcceptor m_Acceptor;
        private readonly SessionManager m_SessionManager;
        private readonly ConcurrentDictionary<long, ClientConnection> m_Connections;
        private readonly IdGenerator m_ClientConnectionIdGenerator;

        private bool m_IsDisposed = false;

        public ClientConnectionManager(SessionManager sessionManager)
        {
            m_Acceptor = new AsyncTcpAcceptor(OnNewConnection);
            m_SessionManager = sessionManager;
            m_Connections = new ConcurrentDictionary<long, ClientConnection>();
            m_ClientConnectionIdGenerator = new IdGenerator();
        }

        public void Initialize(IPAddress ipAddress, int port, int numberOfBacklogSocket)
        {
            m_Acceptor.Bind(ipAddress, port);
            m_Acceptor.ListenAndStart(numberOfBacklogSocket);
        }

        public async Task Destroy()
        {
            m_Acceptor.Dispose();

            const int maxRetryCount = 5;
            for(int i = 0; i < maxRetryCount; i++)
            {
                if (m_Connections.IsEmpty)
                {
                    break;
                }

                foreach(var(id, connection) in m_Connections)
                {
                    connection.Dispose();
                    m_Connections.TryRemove(id, out var temp);
                }

                const int delayTime = 30;
                await Task.Delay(TimeSpan.FromMilliseconds(delayTime));
            }

            Dispose();
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
            long id = m_ClientConnectionIdGenerator.Generate();
            var connection = new ClientConnection(id, socket, m_SessionManager);
            m_Connections.TryAdd(id, connection);
        }
    }
}
