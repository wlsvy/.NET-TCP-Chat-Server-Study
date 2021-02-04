using Server.DataSource;
using Shared.Logger;
using Shared.Protocol;
using System;

namespace Server.Network
{
    public sealed class CSPacketHandler : ICSPacketHandler
    {
        private readonly ClientConnection m_Connection;
        private readonly SessionManager m_SessionManager;

        public CSPacketHandler(ClientConnection connection, SessionManager sessionManager)
        {
            m_Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            m_SessionManager = sessionManager ?? throw new ArgumentNullException(nameof(connection));
        }

        public void HANDLE_CS_Ping(long sequenceNumber)
        {
            Log.I.Info($"Network Ping : {sequenceNumber}");
            m_Connection.PacketSender.SEND_SC_Pong(sequenceNumber);
        }

        public void HANDLE_CS_Login(string id, string password)
        {
            Log.I.Info($"id {id}, password {password}");
            m_Connection.PacketSender.SEND_SC_Login(GlobalDataSource.I.Account.LoginAccount(id, password));
        }

        public void HANDLE_CS_CreateAccount(string id, string password)
        {
            Log.I.Info($"id {id}, password {password}");
            m_Connection.PacketSender.SEND_SC_Login(GlobalDataSource.I.Account.LoginAccount(id, password));
        }

        public void HANDLE_CS_ChatMessage(string message)
        {

        }
    }
}
