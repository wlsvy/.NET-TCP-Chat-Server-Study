using Shared.Logger;
using Shared.Network;
using Shared.Protocol;
using System;
using System.Net.Sockets;

namespace Server.Core
{
    public sealed class ClientConnection : IDisposable
    {
        public readonly long Id;
        private readonly AsyncTcpConnection m_Connection;
        private readonly PacketProcessor m_PacketProcessor;
        private Session m_Session;
        private bool m_IsDisposed = false;

        public ClientConnection(long id, Socket tcpSocket, SessionManager sessionManager)
        {
            _ = tcpSocket ?? throw new ArgumentNullException(nameof(tcpSocket));
            if (tcpSocket.Connected == false)
            {
                throw new ArgumentException(nameof(tcpSocket));
            }

            Id = id;
            m_Connection = new AsyncTcpConnection(tcpSocket);
            m_Connection.Subscribe(
                onReceived: HandleReceivedData,
                onError: error =>
                {
                    Log.I.Error($"패킷 송신 중 오류 발생", error);
                    Dispose();
                },
                onReceiveCompleted: () => Dispose());

            var packetHandler = new ServerPacketHandler();
            m_PacketProcessor = new PacketProcessor(packetHandler);
        }

        public void Send(ArraySegment<byte> data)
        {
            m_Connection.Send(data);
        }

        public void BoundToSession(Session session)
        {
            m_Session = session ?? throw new ArgumentNullException(nameof(session));
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
