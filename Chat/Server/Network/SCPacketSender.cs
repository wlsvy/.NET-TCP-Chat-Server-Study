﻿using Shared.Network;
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
            var packet = PacketPacker.Pack_SC_Ping_NTF(sequenceNumber);
            m_Connection.Send(packet);
        }

        public void SEND_CS_Login_REQ(long accountId)
        {
            var packet = PacketPacker.Pack_SC_Login_RSP(accountId);
            m_Connection.Send(packet);
        }

        public void SEND_CS_CreateAccount_REQ(long accountId)
        {
            var packet = PacketPacker.Pack_SC_CreateAccount_RSP(accountId);
            m_Connection.Send(packet);
        }
    }
}