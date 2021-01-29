﻿using Shared.Network;
using System;
using System.Diagnostics;

namespace Shared.Protocol
{
    public sealed class SCPacketProcessor : PacketProcessorBase
    {
        private readonly ISCPacketHandler m_PacketHandler;

        public SCPacketProcessor(ISCPacketHandler handler)
        {
            m_PacketHandler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public int ParseAndHandlePacket(ArraySegment<byte> dataStream)
        {
            if (!HasPacketHeader(dataStream))
            {
                return 0;
            }

            var packetHeader = ParseHeader(dataStream);
            if (!HasPacketBody(dataStream, packetHeader))
            {
                return 0;
            }

            var packetBody = PeekPacketBody(dataStream, packetHeader);
            ParseAndHandleBody(packetHeader, packetBody);

            return (SCPacketHeader.HEADER_SIZE + packetHeader.BodySize);
        }

        private void ParseAndHandleBody(SCPacketHeader header, ArraySegment<byte> body)
        {
            switch (header.Protocol)
            {
                case SCPacketProtocol.SC_Pong: ParseAndHandle_SC_Pong(body); break;
                case SCPacketProtocol.SC_Login: ParseAndHandle_SC_Login(body); break;
                case SCPacketProtocol.SC_CreateAccount: ParseAndHandle_SC_CreateAccount(body); break;
            }
        }

        #region Packet Paser Method

        private static SCPacketHeader ParseHeader(ArraySegment<byte> dataStream)
        {
            Debug.Assert(dataStream.Array != null);
            Debug.Assert(HasPacketHeader(dataStream));

            int number = BitConverter.ToInt32(dataStream.Array, dataStream.Offset);
            var protocol = SCPacketProtocol.Invalid;

            try
            {
                protocol = (SCPacketProtocol)number;
            }
            catch (InvalidCastException)
            {
                throw;
            }

            int bodySize = BitConverter.ToInt32(dataStream.Array, dataStream.Offset + sizeof(int));
            if (bodySize < 0)
            {
                throw new ArgumentOutOfRangeException($"패킷 BodySize가 음수입니다.핵유저로 의심되네요.ProtocolNumber[{number}] BodySize[{bodySize}]");
            }

            return new SCPacketHeader(protocol, bodySize);
        }

        private static bool HasPacketHeader(ArraySegment<byte> dataStream)
        {
            return (dataStream.Count >= SCPacketHeader.HEADER_SIZE);
        }
        private static bool HasPacketBody(ArraySegment<byte> dataStream, SCPacketHeader packetHeader)
        {
            return (dataStream.Count >= (SCPacketHeader.HEADER_SIZE + packetHeader.BodySize));
        }
        private static ArraySegment<byte> PeekPacketBody(ArraySegment<byte> dataStream, SCPacketHeader packetHeader)
        {
            return new ArraySegment<byte>(dataStream.Array, dataStream.Offset + SCPacketHeader.HEADER_SIZE, packetHeader.BodySize);
        }

        #endregion

        #region Packet handler Method

        private void ParseAndHandle_SC_Pong(ArraySegment<byte> body)
        {
            using (var reader = new BinaryDecoder(body))
            {
                reader.Read(out long sequenceNumber);

                RunOrReserveHandler(handler: async () =>
                {
                    m_PacketHandler.HANDLE_SC_Pong(sequenceNumber);
                });
            }
        }

        private void ParseAndHandle_SC_Login(ArraySegment<byte> body)
        {
            using (var reader = new BinaryDecoder(body))
            {
                reader.Read(out long accountId);

                RunOrReserveHandler(handler: async () =>
                {
                    m_PacketHandler.HANDLE_SC_Login(accountId);
                });
            }
        }

        private void ParseAndHandle_SC_CreateAccount(ArraySegment<byte> body)
        {
            using (var reader = new BinaryDecoder(body))
            {
                reader.Read(out long accountId);

                RunOrReserveHandler(handler: async () =>
                {
                    m_PacketHandler.HANDLE_SC_CreateAccount(accountId);
                });
            }
        }

        #endregion
    }
}
