using System;

namespace Shared.Protocol
{
    public sealed class PacketProcessor
    {

        private void ParseAndHandleBody(PacketHeader header, ArraySegment<byte> body)
        {
            switch (header.Protocol)
            {
                case PacketProtocol.SC_Ping: return;
            }
        }
    }
}
