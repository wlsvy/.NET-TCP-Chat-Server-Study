using Shared.Logger;
using Shared.Network;
using Shared.Protocol;
using Shared.Util;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Client.Network
{
    public sealed class ServerConnection : Singleton<ServerConnection>, IDisposable, ISCPacketHandler
    {
        private AsyncTcpConnection m_Connection;
        private SCPacketProcessor m_PacketProcessor;
        private CSPacketSender m_PacketSender;
        public CSPacketSender PacketSender => m_PacketSender;

        private long m_AccountId = -1;
        private bool IsLogin => m_AccountId != -1;

        private bool m_IsDisposed;

        public void OnConnected(Socket tcpSocket)
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

        #region PacketHandler Callback

        public event Action<long> LoginEvent;
        public event Action<long> CreateAccountEvent;

        #endregion

        #region Packet Handler

        void ISCPacketHandler.HANDLE_SC_Pong(long sequenceNumber)
        {
            Log.I.Debug($"Network Pong : {sequenceNumber}");
        }

        void ISCPacketHandler.HANDLE_SC_Login(long accountId)
        {
            if (accountId == -1)
            {
                Log.I.Debug($"로그인 실패");
            }

            ClientJobManager.I.ReserveJob(async () =>
            {
                m_AccountId = accountId;
                LoginEvent?.Invoke(accountId);
            });
        }

        void ISCPacketHandler.HANDLE_SC_CreateAccount(long accountId)
        {
            if (accountId == -1)
            {
                Log.I.Debug($"계정 생성실패");
            }

            ClientJobManager.I.ReserveJob(async () =>
            {
                m_AccountId = accountId;
                CreateAccountEvent?.Invoke(accountId);
            });
        }

        void ISCPacketHandler.HANDLE_SC_ChatMessage()
        {

        }

        #endregion
    }
}
