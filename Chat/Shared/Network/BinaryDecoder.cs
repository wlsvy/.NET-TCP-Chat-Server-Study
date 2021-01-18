using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Shared.Util;

namespace Shared.Network
{
    public class BinaryDecoder : IDisposable
    {
        private readonly ArraySegment<byte> m_Buffer;
        private int m_ReadOffset;
        public int LeftBytesToRead => (m_Buffer.Count - m_ReadOffset);

        private readonly MemoryStream m_Stream;
        private readonly BinaryReader m_Reader;

        private bool m_IsDisposed;

        public BinaryDecoder(ArraySegment<byte> buffer)
        {
            _ = buffer.Array ?? throw new ArgumentNullException(nameof(buffer));

            m_Buffer = buffer;
            m_Stream = new MemoryStream(m_Buffer.Array, m_Buffer.Offset, m_Buffer.Count);
            m_Reader = new BinaryReader(m_Stream);
        }

        public void Dispose()
        {
            if (m_IsDisposed)
            {
                return;
            }
            m_IsDisposed = true;

            m_Reader.Dispose();
            m_Stream.Dispose();
        }

        public int Read(out short variable)
        {
            variable = m_Reader.ReadInt16();

            int bytesCount = variable.SizeForWrite();
            m_ReadOffset += bytesCount;

            return bytesCount;
        }

        public int Read(out int variable)
        {
            variable = m_Reader.ReadInt32();

            int bytesCount = variable.SizeForWrite();
            m_ReadOffset += bytesCount;

            return bytesCount;
        }

        public int Read(out long variable)
        {
            variable = m_Reader.ReadInt64();

            int bytesCount = variable.SizeForWrite();
            m_ReadOffset += bytesCount;

            return bytesCount;
        }

        public int Read(out ushort variable)
        {
            variable = m_Reader.ReadUInt16();

            int bytesCount = variable.SizeForWrite();
            m_ReadOffset += bytesCount;

            return bytesCount;
        }

        public int Read(out uint variable)
        {
            variable = m_Reader.ReadUInt32();

            int bytesCount = variable.SizeForWrite();
            m_ReadOffset += bytesCount;

            return bytesCount;
        }

        public int Read(out ulong variable)
        {
            variable = m_Reader.ReadUInt64();

            int bytesCount = variable.SizeForWrite();
            m_ReadOffset += bytesCount;

            return bytesCount;
        }

        public int Read(out bool variable)
        {
            variable = m_Reader.ReadBoolean();

            int bytesCount = variable.SizeForWrite();
            m_ReadOffset += bytesCount;

            return bytesCount;
        }

        public int Read(out byte variable)
        {
            variable = m_Reader.ReadByte();

            int bytesCount = variable.SizeForWrite();
            m_ReadOffset += bytesCount;

            return bytesCount;
        }

        public int Read(out float variable)
        {
            variable = m_Reader.ReadSingle();

            int bytesCount = variable.SizeForWrite();
            m_ReadOffset += bytesCount;

            return bytesCount;
        }

        public int Read(out double variable)
        {
            variable = m_Reader.ReadDouble();

            int bytesCount = variable.SizeForWrite();
            m_ReadOffset += bytesCount;

            return bytesCount;
        }

        public int Read(out string variable)
        {
            int length = m_Reader.ReadInt32();
            int bytesCount = length.SizeForWrite();

            Debug.Assert((m_ReadOffset + sizeof(int) + length) <= m_Buffer.Count);
            var bytes = m_Reader.ReadBytes(length);
            bytesCount += length;
            m_ReadOffset += bytesCount;

            variable = Encoding.UTF8.GetString(bytes);

            return bytesCount;
        }
        public ArraySegment<byte> ReadBytes(int count)
        {
            Debug.Assert((m_ReadOffset + count) <= m_Buffer.Count);
            byte[] value = m_Reader.ReadBytes(count);
            m_ReadOffset += count;

            return new ArraySegment<byte>(value);
        }

        public int Read(out List<int> list)
        {
            int totalBytes = this.Read(out int length);

            list = new List<int>(length);

            for (int i = 0; i < length; ++i)
            {
                totalBytes += this.Read(out int element);
                list.Add(element);
            }

            return totalBytes;
        }

        public int Read(out List<string> list)
        {
            int totalBytes = this.Read(out int length);

            list = new List<string>(length);

            for (int i = 0; i < length; ++i)
            {
                totalBytes += this.Read(out string element);
                list.Add(element);
            }

            return totalBytes;
        }
    }
}
