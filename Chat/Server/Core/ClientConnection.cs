using System;
using System.Net.Sockets;

namespace Server.Core
{
    public sealed class ClientConnection : IDisposable
    {
        public readonly long Id;
        //public readonly SCPacketSender PacketSender;
        private Session m_Session;

        private bool m_IsDisposed = false;

        public ClientConnection(long id, Socket tcpSocket, SessionManager sessionManager)
        {
            if(tcpSocket == null ||
                tcpSocket.Connected == false)
            {
                throw new ArgumentException(nameof(tcpSocket));
            }

            Id = id;
            //PacketSender = new SCPacketSender();
        }

        public void StartReceive()
        {

        }

        public void Send()
        {

        }

        public void BoundToSession(Session session)
        {
            if(session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }
            m_Session = session;
        }

        public void Dispose()
        {
            if (m_IsDisposed)
            {
                return;
            }
            m_IsDisposed = true;

            m_Session.OnDisconnected();
        }
    }
}
