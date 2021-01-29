using Shared.Network;
using System;
using Shared.Util;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Protocol
{
    internal static class CSPacketPacker
    {
        private static void WriteHeader(BinaryEncoder encoder, CSPacketProtocol protocol, int bodySize)
        {
            encoder.Write(in protocol);
            encoder.Write(in bodySize);
        }

        public static ArraySegment<byte> Pack_CS_Pong_NTF(long sequenceNum)
        {
            var protocol = CSPacketProtocol.CS_Ping;

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
            var protocol = CSPacketProtocol.CS_Login;

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
            var protocol = CSPacketProtocol.CS_CreateAccount;

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
    }
}
