using Shared.Logger;
using Shared.Network;
using Shared.Protocol;
using System;

namespace Client
{
    public sealed class ClientPacketProcessor : PacketProcessorBase, ISCPacketHandler
    {
        protected override void ParseAndHandleBody(PacketHeader header, ArraySegment<byte> body)
        {
            switch (header.Protocol)
            {
                case PacketProtocol.SC_Ping_NTF: ParseAndHandle_SC_Ping_NTF(body); break;
                case PacketProtocol.SC_Login_RSP: ParseAndHandle_SC_Login_RSP(body); break;
                case PacketProtocol.SC_CreateAccount_RSP: ParseAndHandle_SC_CreateAccount_RSP(body); break;
            }
        }

        private void ParseAndHandle_SC_Ping_NTF(ArraySegment<byte> body)
        {
            using (var reader = new BinaryDecoder(body))
            {
                reader.Read(out long sequenceNumber);

                RunOrReserveHandler(handler: async () =>
                {
                    HANDLE_SC_Ping_NTF(sequenceNumber);
                });
            }
        }

        private void ParseAndHandle_SC_Login_RSP(ArraySegment<byte> body)
        {
            using (var reader = new BinaryDecoder(body))
            {
                reader.Read(out long accountId);

                RunOrReserveHandler(handler: async () =>
                {
                    HANDLE_SC_Login_RSP(accountId);
                });
            }
        }

        private void ParseAndHandle_SC_CreateAccount_RSP(ArraySegment<byte> body)
        {
            using (var reader = new BinaryDecoder(body))
            {
                reader.Read(out long accountId);

                RunOrReserveHandler(handler: async () =>
                {
                    HANDLE_SC_CreateAccount_RSP(accountId);
                });
            }
        }

        private void HANDLE_SC_Ping_NTF(long sequenceNumber)
        {
            Log.I.Info($"Network Pong : {sequenceNumber}");
        }

        private void HANDLE_SC_Login_RSP(long accountId)
        {
            Log.I.Info($"Network HANDLE_SC_Login_RSP : {accountId}");
        }

        private void HANDLE_SC_CreateAccount_RSP(long accountId)
        {
            Log.I.Info($"Network HANDLE_SC_CreateAccount_RSP : {accountId}");
        }
    }
}
