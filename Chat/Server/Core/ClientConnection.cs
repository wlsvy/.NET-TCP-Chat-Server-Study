using Shared.Logger;
using Shared.Network;
using System;
using System.Net.Sockets;

namespace Server.Core
{
    public sealed class ClientConnection : IDisposable
    {
        public readonly long Id;
        //public readonly SCPacketSender PacketSender;
        private Session m_Session;
        private AsyncTcpConnection m_Connection;
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

            m_Connection.Subscribe(
                onReceived: HandleReceivedData,
                onError: error =>
                {
                    Log.I.Error($"패킷 송신 중 오류 발생", error);
                    Dispose();
                },
                onReceiveCompleted: () => Dispose());
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

        private int HandleReceivedData(ArraySegment<byte> receivedBytes)
        {
            int totalConsumedByte = 0;

            try
            {
                int consumedBytes = 0;
                var leftBuffer = new ArraySegment<byte>(
                    receivedBytes.Array, 
                    receivedBytes.Offset + totalConsumedByte,
                    receivedBytes.Count - totalConsumedByte);
            }
            catch(Exception e)
            {
                Log.I.Error($"패킷 처리 중 오류가 발생했습니다.", e);
                Dispose();
                return -1;
            }
            return totalConsumedByte;
        }
    }
}
