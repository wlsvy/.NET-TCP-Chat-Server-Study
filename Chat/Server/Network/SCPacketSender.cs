using Shared.Network;
using Shared.Protocol;
using System;

namespace Server.Network
{
    public sealed class SCPacketSender
    {
        private AsyncTcpConnection m_Connection;

        public SCPacketSender(AsyncTcpConnection connection)
        {
            m_Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public void SEND_CS_Pong_NTF(long sequenceNumber)
        {
            var packet = SCPacketPacker.Pack_SC_Ping(sequenceNumber);
            m_Connection.Send(packet);
        }

        public void SEND_CS_Login_REQ(long accountId)
        {
            var packet = SCPacketPacker.Pack_SC_Login(accountId);
            m_Connection.Send(packet);
        }

        public void SEND_CS_CreateAccount_REQ(long accountId)
        {
            var packet = SCPacketPacker.Pack_SC_CreateAccount(accountId);
            m_Connection.Send(packet);
        }
    }
}
