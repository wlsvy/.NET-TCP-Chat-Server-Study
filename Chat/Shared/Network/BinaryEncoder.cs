using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Shared.Util;

namespace Shared.Network
{
    public class BinaryEncoder : IDisposable
    {
        private readonly ArraySegment<byte> m_Buffer;
        private int m_WrittenBytes;
        private int LeftBufferSize => m_Buffer.Count - m_WrittenBytes;

        private readonly MemoryStream m_Stream;
        private readonly BinaryWriter m_Writer;

        private bool m_IsDispoed;

        public BinaryEncoder(ArraySegment<byte> buffer)
        {
            _ = buffer.Array ?? throw new ArgumentNullException(nameof(buffer));

            m_Buffer = buffer;
            m_Stream = new MemoryStream(m_Buffer.Array, m_Buffer.Offset, m_Buffer.Count);
            m_Writer = new BinaryWriter(m_Stream);
        }

        public void Dispose()
        {
            if (m_IsDispoed)
            {
                return;
            }
            m_IsDispoed = true;

            Debug.Assert(m_Buffer.Count == m_WrittenBytes);

            m_Writer.Dispose();
            m_Stream.Dispose();
        }

        public void Write(in short value)
        {
            int bytesCount = value.SizeForWrite();
            Debug.Assert(LeftBufferSize >= bytesCount);

            m_Writer.Write(value);

            m_WrittenBytes += bytesCount;
        }

        public void Write(in int value)
        {
            int bytesCount = value.SizeForWrite();
            Debug.Assert(LeftBufferSize >= bytesCount);

            m_Writer.Write(value);

            m_WrittenBytes += bytesCount;
        }

        public void Write(in long value)
        {
            int bytesCount = value.SizeForWrite();
            Debug.Assert(LeftBufferSize >= bytesCount);

            m_Writer.Write(value);

            m_WrittenBytes += bytesCount;
        }

        public void Write(in ushort value)
        {
            int bytesCount = value.SizeForWrite();
            Debug.Assert(LeftBufferSize >= bytesCount);

            m_Writer.Write(value);

            m_WrittenBytes += bytesCount;
        }

        public void Write(in uint value)
        {
            int bytesCount = value.SizeForWrite();
            Debug.Assert(LeftBufferSize >= bytesCount);

            m_Writer.Write(value);

            m_WrittenBytes += bytesCount;
        }

        public void Write(in ulong value)
        {
            int bytesCount = value.SizeForWrite();
            Debug.Assert(LeftBufferSize >= bytesCount);

            m_Writer.Write(value);

            m_WrittenBytes += bytesCount;
        }

        public void Write(in bool value)
        {
            int bytesCount = value.SizeForWrite();
            Debug.Assert(LeftBufferSize >= bytesCount);

            m_Writer.Write(value);

            m_WrittenBytes += bytesCount;
        }

        public void Write(in byte value)
        {
            int bytesCount = value.SizeForWrite();
            Debug.Assert(LeftBufferSize >= bytesCount);

            m_Writer.Write(value);

            m_WrittenBytes += bytesCount;
        }

        public void Write(in float value)
        {
            int bytesCount = value.SizeForWrite();
            Debug.Assert(LeftBufferSize >= bytesCount);

            m_Writer.Write(value);

            m_WrittenBytes += bytesCount;
        }

        public void Write(in double value)
        {
            int bytesCount = value.SizeForWrite();
            Debug.Assert(LeftBufferSize >= bytesCount);

            m_Writer.Write(value);

            m_WrittenBytes += bytesCount;
        }

        public void Write(in string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);

            int length = bytes.Length;

            int bytesCount = length.SizeForWrite() +
                             length;
            Debug.Assert(value.SizeForWrite() == bytesCount);
            Debug.Assert(LeftBufferSize >= bytesCount);

            m_Writer.Write(bytes.Length);
            m_Writer.Write(bytes);

            m_WrittenBytes += bytesCount;
        }

        public void Write(ArraySegment<byte> bytes)
        {
            Debug.Assert(LeftBufferSize >= bytes.Count);

            m_Writer.Write(bytes.Array, bytes.Offset, bytes.Count);

            m_WrittenBytes += bytes.Count;
        }

        public void Write(in List<int> list)
        {
            int length = list.Count;
            this.Write(in length);

            foreach (var element in list)
            {
                this.Write(element);
            }
        }

        public void Write(in List<string> list)
        {
            int length = list.Count;
            this.Write(in length);

            foreach (var element in list)
            {
                // foreach element에 ref를 사용할 수 없어 스택을 사용한다.
                this.Write(in element);
            }
        }
    }
}
