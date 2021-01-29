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


        public static ArraySegment<byte> Pack_CS_Pong_NTF(long sequenceNum)
        {
            var protocol = PacketProtocol.CS_Pong_NTF;

            int bodySize = sequenceNum.SizeForWrite();
            int packetSize = CSPacketHeader.HEADER_SIZE + bodySize;

            var packetBuffer = new ArraySegment<byte>(new byte[packetSize]);
            using (var encoder = new BinaryEncoder(packetBuffer))
            {
                WriteHeader(encoder, protocol, bodySize);
                encoder.Write(in sequenceNum);
            }
            return packetBuffer;
        }

        public static ArraySegment<byte> Pack_CS_Login_REQ(string id, string password)
        {
            var protocol = PacketProtocol.CS_Login_REQ;

            int bodySize = 
                id.SizeForWrite() + 
                password.SizeForWrite();
            int packetSize = CSPacketHeader.HEADER_SIZE + bodySize;

            var packetBuffer = new ArraySegment<byte>(new byte[packetSize]);
            using (var encoder = new BinaryEncoder(packetBuffer))
            {
                WriteHeader(encoder, protocol, bodySize);
                encoder.Write(in id);
                encoder.Write(in password);
            }
            return packetBuffer;
        }


        public static ArraySegment<byte> Pack_CS_CreateAccount_REQ(string id, string password)
        {
            var protocol = PacketProtocol.CS_CreateAccount_REQ;

            int bodySize =
               id.SizeForWrite() +
               password.SizeForWrite();
            int packetSize = CSPacketHeader.HEADER_SIZE + bodySize;

            var packetBuffer = new ArraySegment<byte>(new byte[packetSize]);
            using (var encoder = new BinaryEncoder(packetBuffer))
            {
                WriteHeader(encoder, protocol, bodySize);
                encoder.Write(in id);
                encoder.Write(in password);
            }
            return packetBuffer;
        }

        public static ArraySegment<byte> Pack_SC_Ping_NTF(long sequenceNum)
        {
            var protocol = PacketProtocol.SC_Ping_NTF;

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
        public static ArraySegment<byte> Pack_SC_Login_RSP(long accountId)
        {
            var protocol = PacketProtocol.SC_Login_RSP;

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

        public static ArraySegment<byte> Pack_SC_CreateAccount_RSP(long accountId)
        {
            var protocol = PacketProtocol.SC_CreateAccount_RSP;

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
