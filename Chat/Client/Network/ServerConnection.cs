using Shared.Logger;
using Shared.Network;
using Shared.Protocol;
using System;
using System.Net.Sockets;

namespace Client.Network
{
    public sealed class ServerConnection : IDisposable, ISCPacketHandler
    {
        private readonly AsyncTcpConnection m_Connection;
        private readonly SCPacketProcessor m_PacketProcessor;
        private readonly CSPacketSender m_PacketSender;
        public CSPacketSender PacketSender => m_PacketSender;

        private long m_AccountId = -1;
        private bool IsLogin => m_AccountId != -1;

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
            m_PacketProcessor = new SCPacketProcessor(this);
            m_PacketSender = new CSPacketSender(m_Connection);
        }

        public void Dispose()
        {
            if (m_IsDisposed)
            {
                return;
            }
            m_IsDisposed = true;

            m_Connection.Dispose();
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

                    consumedBytes = m_PacketProcessor.ParseAndHandlePacket(leftBuffer);
                    totalConsumedByte += consumedBytes;
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

        #region Packet Handler

        public void HANDLE_SC_Pong(long sequenceNumber)
        {
            Log.I.Debug($"Network Pong : {sequenceNumber}");
        }

        public void HANDLE_SC_Login(long accountId)
        {
            if (accountId == -1)
            {
                Log.I.Debug($"로그인 실패");
                return;
            }

            Log.I.Debug($"로그인 성공, AccountId : {accountId}");
            m_AccountId = accountId;
        }

        public void HANDLE_SC_CreateAccount(long accountId)
        {
            if (accountId == -1)
            {
                Log.I.Debug($"계정 생성실패");
                return;
            }

            Log.I.Debug($"계정 생성 성공, AccountId : {accountId}");
            m_AccountId = accountId;
        }

        public void HANDLE_SC_ChatMessage()
        {

        }

        #endregion
    }
}
