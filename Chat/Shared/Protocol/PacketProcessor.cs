using Shared.Interface;
using Shared.Network;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Shared.Protocol
{
    public sealed class PacketProcessor
    {
        private ConcurrentQueue<Func<Task>> m_HandlerQueue;
        private IPacketHandler m_PacketHandler;

        public PacketProcessor(IPacketHandler handler)
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

            return (PacketHeader.HEADER_SIZE + packetHeader.BodySize);
        }

        private static PacketHeader ParseHeader(ArraySegment<byte> dataStream)
        {
            Debug.Assert(dataStream.Array != null);
            Debug.Assert(HasPacketHeader(dataStream));

            int number = BitConverter.ToInt32(dataStream.Array, dataStream.Offset);
            var protocol = PacketProtocol.Invalid;

            try
            {
                protocol = (PacketProtocol)number;
            }
            catch (InvalidCastException)
            {
                throw;
            }

            int bodySize = BitConverter.ToInt32(dataStream.Array, dataStream.Offset + sizeof(int));
            if (bodySize < 0)
            {
                throw new InvalidDataException($"패킷 BodySize가 음수입니다.핵유저로 의심되네요.ProtocolNumber[{number}] BodySize[{bodySize}]");
            }

            return new PacketHeader(protocol, bodySize);
        }

        private static bool HasPacketHeader(ArraySegment<byte> dataStream)
        {
            return (dataStream.Count >= PacketHeader.HEADER_SIZE);
        }
        private static bool HasPacketBody(ArraySegment<byte> dataStream, PacketHeader packetHeader)
        {
            return (dataStream.Count >= (PacketHeader.HEADER_SIZE + packetHeader.BodySize));
        }
        private static ArraySegment<byte> PeekPacketBody(ArraySegment<byte> dataStream, PacketHeader packetHeader)
        {
            return new ArraySegment<byte>(dataStream.Array, dataStream.Offset + PacketHeader.HEADER_SIZE, packetHeader.BodySize);
        }

        private void ParseAndHandleBody(PacketHeader header, ArraySegment<byte> body)
        {
            switch (header.Protocol)
            {
                case PacketProtocol.SC_Ping: return;
            }
        }

        private void ParseAndHandle_SC_Ping(ArraySegment<byte> body)
        {
            using (var reader = new BinaryDecoder(body))
            {
                reader.Read(out long sequenceNumber);

                ReserveHandler(handler: async () =>
                {
                    m_PacketHandler.HANDLE_SC_Ping(sequenceNumber);
                });
            }
        }

        private void ReserveHandler(Func<Task> handler)
        {
            m_HandlerQueue.Enqueue(handler);
        }
    }
}
