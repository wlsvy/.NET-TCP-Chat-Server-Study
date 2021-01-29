using Shared.Network;
using Shared.Util;
using System;

namespace Shared.Protocol
{
    internal class SCPacketPacker
    {
        private static void WriteHeader(BinaryEncoder encoder, SCPacketProtocol protocol, int bodySize)
        {
            encoder.Write(in protocol);
            encoder.Write(in bodySize);
        }

        public static ArraySegment<byte> Pack_SC_Ping(long sequenceNum)
        {
            var protocol = SCPacketProtocol.SC_Pong;

            int bodySize = sequenceNum.SizeForWrite();
            int packetSize = SCPacketHeader.HEADER_SIZE + bodySize;

            var packetBuffer = new ArraySegment<byte>(new byte[packetSize]);
            using (var encoder = new BinaryEncoder(packetBuffer))
            {
                WriteHeader(encoder, protocol, bodySize);
                encoder.Write(in sequenceNum);
            }
            return packetBuffer;
        }
        public static ArraySegment<byte> Pack_SC_Login(long accountId)
        {
            var protocol = SCPacketProtocol.SC_Login;

            int bodySize = accountId.SizeForWrite();
            int packetSize = SCPacketHeader.HEADER_SIZE + bodySize;

            var packetBuffer = new ArraySegment<byte>(new byte[packetSize]);
            using (var encoder = new BinaryEncoder(packetBuffer))
            {
                WriteHeader(encoder, protocol, bodySize);
                encoder.Write(in accountId);
            }
            return packetBuffer;
        }

        public static ArraySegment<byte> Pack_SC_CreateAccount(long accountId)
        {
            var protocol = SCPacketProtocol.SC_CreateAccount;

            int bodySize = accountId.SizeForWrite();
            int packetSize = SCPacketHeader.HEADER_SIZE + bodySize;

            var packetBuffer = new ArraySegment<byte>(new byte[packetSize]);
            using (var encoder = new BinaryEncoder(packetBuffer))
            {
                WriteHeader(encoder, protocol, bodySize);
                encoder.Write(in accountId);
            }
            return packetBuffer;
        }
    }
}
