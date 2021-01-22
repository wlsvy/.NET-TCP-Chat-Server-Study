using Shared.Logger;
using Shared.Network;
using Shared.Protocol;
using System;

namespace Client
{
    public sealed class ClientPacketProcessor : PacketProcessorBase
    {
        protected override void ParseAndHandleBody(PacketHeader header, ArraySegment<byte> body)
        {
            switch (header.Protocol)
            {
                case PacketProtocol.SC_Ping_NTF: ParseAndHandle_SC_Ping_NTF(body); break;
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

        protected override void HANDLE_SC_Ping_NTF(long sequenceNumber)
        {
            Log.I.Info($"Network Pong : {sequenceNumber}");
        }
    }
}
