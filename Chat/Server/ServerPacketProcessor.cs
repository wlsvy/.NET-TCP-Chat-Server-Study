using Shared.Logger;
using Shared.Network;
using Shared.Protocol;
using System;

namespace Server
{
    public sealed class ServerPacketProcessor : PacketProcessorBase
    {
        protected override void ParseAndHandleBody(PacketHeader header, ArraySegment<byte> body)
        {
            switch (header.Protocol)
            {
                case PacketProtocol.CS_Pong_NTF: ParseAndHandle_CS_Pong_NTF(body); break;
            }
        }

        private void ParseAndHandle_CS_Pong_NTF(ArraySegment<byte> body)
        {
            using (var reader = new BinaryDecoder(body))
            {
                reader.Read(out long sequenceNumber);

                RunOrReserveHandler(handler: async () =>
                {
                    HANDLE_CS_Pong_NTF(sequenceNumber);
                });
            }
        }

        protected override void HANDLE_CS_Pong_NTF(long sequenceNumber)
        {
            Log.I.Info($"Network Ping : {sequenceNumber}");
        }
    }
}
