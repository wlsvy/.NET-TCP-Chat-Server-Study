using Shared.Logger;
using Shared.Network;
using Shared.Protocol;
using System;

namespace Server.Network
{
    public sealed class CSPacketProcessor : PacketProcessorBase, ICSPacketHandler
    {
        protected override void ParseAndHandleBody(PacketHeader header, ArraySegment<byte> body)
        {
            switch (header.Protocol)
            {
                case PacketProtocol.CS_Pong_NTF: ParseAndHandle_CS_Pong_NTF(body); break;
                case PacketProtocol.CS_Login_REQ: ParseAndHandle_CS_Login_REQ(body); break;
                case PacketProtocol.CS_CreateAccount_REQ: ParseAndHandle_CS_CreateAccount_REQ(body); break;
            }
        }

        private void ParseAndHandle_CS_Pong_NTF(ArraySegment<byte> body)
        {
            using (var reader = new BinaryDecoder(body))
            {
                reader.Read(out long sequenceNumber);

                RunOrReserveHandler(handler: async () =>
                {
                    HANDLE_CS_Ping(sequenceNumber);
                });
            }
        }

        private void ParseAndHandle_CS_Login_REQ(ArraySegment<byte> body)
        {
            using (var reader = new BinaryDecoder(body))
            {
                reader.Read(out string id);
                reader.Read(out string password);

                RunOrReserveHandler(handler: async () =>
                {
                    HANDLE_CS_Login(id, password);
                });
            }
        }

        private void ParseAndHandle_CS_CreateAccount_REQ(ArraySegment<byte> body)
        {
            using (var reader = new BinaryDecoder(body))
            {
                reader.Read(out string id);
                reader.Read(out string password);

                RunOrReserveHandler(handler: async () =>
                {
                    HANDLE_CS_CreateAccount(id, password);
                });
            }
        }

        public void HANDLE_CS_Ping(long sequenceNumber)
        {
            Log.I.Info($"Network Ping : {sequenceNumber}");
        }

        public void HANDLE_CS_Login(string id, string password)
        {
            Log.I.Info($"id {id}, password {password}");
        }

        public void HANDLE_CS_CreateAccount(string id, string password)
        {
            Log.I.Info($"id {id}, password {password}");
        }

        public void HANDLE_CS_ChatMessage(string message)
        {

        }
    }
}
