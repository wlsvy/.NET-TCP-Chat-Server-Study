using Shared.Network;
using Shared.Protocol;
using System;

namespace Client.Network
{
    public sealed class CSPacketSender
    {
        private AsyncTcpConnection m_Connection;

        public CSPacketSender(AsyncTcpConnection connection)
        {
            m_Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public void SEND_CS_Pong_NTF(long sequenceNumber)
        {
            var packet = PacketPacker.Pack_CS_Pong_NTF(sequenceNumber);
            m_Connection.Send(packet);
        }

        public void SEND_CS_Login_REQ(string id, string password)
        {
            var packet = PacketPacker.Pack_CS_Login_REQ(id, password);
            m_Connection.Send(packet);
        }

        public void SEND_CS_CreateAccount_REQ(string id, string password)
        {
            var packet = PacketPacker.Pack_CS_CreateAccount_REQ(id, password);
            m_Connection.Send(packet);
        }
    }
}
