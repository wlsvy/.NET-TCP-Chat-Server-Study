﻿//======================================
//======       AutoGenerated       =====
//======================================

using Shared.Network;
using System;
using System.Diagnostics;

namespace Shared.Protocol
{
    public sealed class CSPacketProcessor : PacketProcessorBase
    {
        private readonly ICSPacketHandler m_PacketHandler;
        
        public CSPacketProcessor(ICSPacketHandler handler)
        {
            m_PacketHandler = handler ?? throw new ArgumentNullException(nameof(handler));
        }
        public int ParseAndHandlePacket(ArraySegment<byte> dataStream)
        {
            if(!HasPacketHeader(dataStream))
            {
                return 0;
            }
            
            var packetHeader = ParseHeader(dataStream);
            if(!HasPacketBody(dataStream, packetHeader))
            {
                return 0;
            }
            
            var packetBody = PeekPacketBody(dataStream, packetHeader);
            ParseAndHandleBody(packetHeader, packetBody);
            
            return CSPacketHeader.HEADER_SIZE + packetHeader.BodySize;
        }
        private void ParseAndHandleBody(CSPacketHeader header, ArraySegment<byte> body)
        {
            switch (header.Protocol)
            {
                case CSPacketProtocol.CS_Ping: ParseAndHandle_CS_Ping(body); break;
                case CSPacketProtocol.CS_Login: ParseAndHandle_CS_Login(body); break;
                case CSPacketProtocol.CS_CreateAccount: ParseAndHandle_CS_CreateAccount(body); break;
                case CSPacketProtocol.CS_ChatMessage: ParseAndHandle_CS_ChatMessage(body); break;
            }
        }
        #region Packet Paser Method
        
        private static CSPacketHeader ParseHeader(ArraySegment<byte> dataStream)
        {
            Debug.Assert(dataStream.Array != null);
            Debug.Assert(HasPacketHeader(dataStream));
            
            int number = BitConverter.ToInt32(dataStream.Array, dataStream.Offset);
            var protocol = CSPacketProtocol.Invalid;
            try
            {
                protocol = (CSPacketProtocol)number;
            }
            catch (InvalidCastException)
            {
                throw;
            }
            
            int bodySize = BitConverter.ToInt32(dataStream.Array, dataStream.Offset + sizeof(int));
            if (bodySize < 0)
            {
                throw new ArgumentOutOfRangeException($"패킷 BodySize가 음수입니다. ProtocolNumber[{number}] BodySize[{bodySize}]");
            }
            
            return new CSPacketHeader(protocol, bodySize);
        }
        private static bool HasPacketHeader(ArraySegment<byte> dataStream)
        {
            return (dataStream.Count >= CSPacketHeader.HEADER_SIZE);
        }
        private static bool HasPacketBody(ArraySegment<byte> dataStream, CSPacketHeader packetHeader)
        {
            return (dataStream.Count >= (CSPacketHeader.HEADER_SIZE + packetHeader.BodySize));
        }
        private static ArraySegment<byte> PeekPacketBody(ArraySegment<byte> dataStream, CSPacketHeader packetHeader)
        {
            return new ArraySegment<byte>(dataStream.Array, dataStream.Offset + CSPacketHeader.HEADER_SIZE, packetHeader.BodySize);
        }
        
        #endregion
        
        #region Packet handler Method
        
        private void ParseAndHandle_CS_Ping(ArraySegment<byte> body)
        {
            using (var reader = new BinaryDecoder(body))
            {
                reader.Read(out long sequenceNumber);
                
                RunOrReserveHandler(handler: async () =>
                {
                    m_PacketHandler.HANDLE_CS_Ping(sequenceNumber);
                });
            }
        }
        private void ParseAndHandle_CS_Login(ArraySegment<byte> body)
        {
            using (var reader = new BinaryDecoder(body))
            {
                reader.Read(out string id);
                reader.Read(out string password);
                
                RunOrReserveHandler(handler: async () =>
                {
                    m_PacketHandler.HANDLE_CS_Login(id, password);
                });
            }
        }
        private void ParseAndHandle_CS_CreateAccount(ArraySegment<byte> body)
        {
            using (var reader = new BinaryDecoder(body))
            {
                reader.Read(out string id);
                reader.Read(out string password);
                
                RunOrReserveHandler(handler: async () =>
                {
                    m_PacketHandler.HANDLE_CS_CreateAccount(id, password);
                });
            }
        }
        private void ParseAndHandle_CS_ChatMessage(ArraySegment<byte> body)
        {
            using (var reader = new BinaryDecoder(body))
            {
                reader.Read(out string message);
                
                RunOrReserveHandler(handler: async () =>
                {
                    m_PacketHandler.HANDLE_CS_ChatMessage(message);
                });
            }
        }
        
        #endregion
    }
}
