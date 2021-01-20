using Shared.Logger;
using Shared.Network;
using Shared.Protocol;
using System;
using System.Net;
using System.Net.Sockets;

namespace Client.Core
{
    public sealed class ServerConnection : IDisposable
    {
        private readonly AsyncTcpConnection m_Connection;
        private readonly PacketProcessorBase m_PacketProcessor;
        public readonly IPAddress ServerIp;
        public readonly EndPoint RemoteEndPoint;
        public readonly Socket ConnectSocket;

        private bool m_IsDisposed;

        public ServerConnection(Socket tcpSocket)
        {
            _ = tcpSocket ?? throw new ArgumentNullException(nameof(tcpSocket));
            if (tcpSocket.Connected == false)
            {
                throw new ArgumentException(nameof(tcpSocket));
            }

            m_Connection = new AsyncTcpConnection(tcpSocket);
            m_Connection.Subscribe(
                onReceived: HandleReceivedData,
                onError: error =>
                {
                    Log.I.Error($"패킷 송신 중 오류 발생", error);
                    Dispose();
                },
                onReceiveCompleted: () => Dispose());

            var packetHandler = new ClientPacketHandler();
            m_PacketProcessor = new PacketProcessorBase(packetHandler);
        }

        public void Send(ArraySegment<byte> data)
        {
            m_Connection.Send(data);
        }

        public void Dispose()
        {
            if (m_IsDisposed)
            {
                return;
            }
            m_IsDisposed = true;

        }

        private int HandleReceivedData(ArraySegment<byte> receivedBytes)
        {
            int totalConsumedByte = 0;

            try
            {
                int consumedBytes = 0;
                do
                {
                    var leftBuffer = new ArraySegment<byte>(
                    receivedBytes.Array,
                    receivedBytes.Offset + totalConsumedByte,
                    receivedBytes.Count - totalConsumedByte);
                } while (consumedBytes > 0);
            }
            catch (Exception e)
            {
                Log.I.Error($"패킷 처리 중 오류가 발생했습니다.", e);
                Dispose();
                return -1;
            }
            return totalConsumedByte;
        }
    }
}
