using Shared.Logger;
using Shared.Network;
using Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Client.Network
{
    public sealed class SCPacketProcessor : PacketProcessorBase, ISCPacketHandler
    {
        private ConcurrentQueue<Func<Task>> m_ReservedNetworkHandler = new ConcurrentQueue<Func<Task>>();

        public async Task Update()
        {
            while(m_ReservedNetworkHandler.TryDequeue(out var handler))
            {
                await handler.Invoke();
            }
        }

        protected override void ParseAndHandleBody(PacketHeader header, ArraySegment<byte> body)
        {
            switch (header.Protocol)
            {
                case PacketProtocol.SC_Ping_NTF: ParseAndHandle_SC_Pong(body); break;
                case PacketProtocol.SC_Login_RSP: ParseAndHandle_SC_Login_RSP(body); break;
                case PacketProtocol.SC_CreateAccount_RSP: ParseAndHandle_SC_CreateAccount_RSP(body); break;
                default: throw new NotImplementedException();
            }
        }

        private void ParseAndHandle_SC_Pong(ArraySegment<byte> body)
        {
            using (var reader = new BinaryDecoder(body))
            {
                reader.Read(out long sequenceNumber);

                m_ReservedNetworkHandler.Enqueue(async () =>
                {
                    HANDLE_SC_Pong(sequenceNumber);
                });
            }
        }

        private void ParseAndHandle_SC_Login_RSP(ArraySegment<byte> body)
        {
            using (var reader = new BinaryDecoder(body))
            {
                reader.Read(out long accountId);

                m_ReservedNetworkHandler.Enqueue(async () =>
                {
                    HANDLE_SC_Login(accountId);
                });
            }
        }

        private void ParseAndHandle_SC_CreateAccount_RSP(ArraySegment<byte> body)
        {
            using (var reader = new BinaryDecoder(body))
            {
                reader.Read(out long accountId);

                m_ReservedNetworkHandler.Enqueue(async () =>
                {
                    HANDLE_SC_CreateAccount(accountId);
                });
            }
        }

        public void HANDLE_SC_Pong(long sequenceNumber)
        {
            Log.I.Info($"Network Pong : {sequenceNumber}");
        }

        public void HANDLE_SC_Login(long accountId)
        {
            Log.I.Info($"Network HANDLE_SC_Login_RSP : {accountId}");
            
        }

        public void HANDLE_SC_CreateAccount(long accountId)
        {
            Log.I.Info($"Network HANDLE_SC_CreateAccount_RSP : {accountId}");
        }

        public void HANDLE_SC_ChatMessage()
        {

        }
    }
}
