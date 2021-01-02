using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

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

        public void Initialize(IPAddress ipAddress, int port, int numberOfBacklogSocket)
        {
            m_Acceptor.Bind(ipAddress, port);
            m_Acceptor.ListenAndStart(numberOfBacklogSocket);
        }

        public async Task Destroy()
        {
            m_Acceptor.Dispose();

            const int maxRetryCount = 5;
            const int delayTime = 30;

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

                await Task.Delay(TimeSpan.FromMilliseconds(delayTime));
            }

            this.Dispose();
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
            long id = GenerateClientConnectionId();
            var connection = new ClientConnection(id, socket, m_SessionManager);

            if(!m_Connections.TryAdd(id, connection))
            {
                throw new Exception($"{nameof(ClientConnectionManager)}.{nameof(OnNewConnection)} 중복되는 {nameof(ClientConnection)} ID가 나타났습니다.");
                connection.Dispose();
                return;
            }

            connection.StartReceive();
            connection.PacketSender.SEND_SYSTEM_TEST_PINT(0);
        }

        private long GenerateClientConnectionId()
        {
            return Interlocked.Increment(ref m_PreviousClientConnectionId);
        }

    }
}
