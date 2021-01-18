using Shared.Network;
using Shared.Util;
using System;

namespace Shared.Protocol
{
    public static class PacketPacker
    {
        private static void WriteHeader(BinaryEncoder encoder, PacketProtocol protocol, int bodySize)
        {
            encoder.Write(in protocol);
            encoder.Write(in bodySize);
        }

        public static ArraySegment<byte> Pack_SC_Ping(long sequenceNum)
        {
            var protocol = PacketProtocol.SC_Ping;

            int bodySize = sequenceNum.SizeForWrite();
            int packetSize = PacketHeader.HEADER_SIZE + bodySize;

            var packetBuffer = new ArraySegment<byte>(new byte[packetSize]);
            using (var encoder = new BinaryEncoder(packetBuffer))
            {
                WriteHeader(encoder, protocol, bodySize);
                encoder.Write(in sequenceNum);
            }
            return packetBuffer;
        }
    }
}
