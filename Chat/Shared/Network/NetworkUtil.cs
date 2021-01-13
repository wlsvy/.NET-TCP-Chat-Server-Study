using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Shared.Network
{
    internal static class NetworkUtil
    {
        internal static void PrepareReceiveBuffer(this SocketAsyncEventArgs receiveContext, int bytesToReceive)
        {
            var receiveContextData = receiveContext.UserToken as ReceiveContextData;

            receiveContextData.CheckAndRearrangeReceiveBuffer(bytesToReceive);
            var receiveBuffer = receiveContextData.ReceiveBuffer;
            int beginning = receiveBuffer.Offset + receiveBuffer.Count;

            if (bytesToReceive > (receiveBuffer.Array.Length - beginning))
            {
                throw new Exception("메모리 버퍼가 충분히 확보되지 않음");
            }
            receiveContext.SetBuffer(receiveBuffer.Array, beginning, bytesToReceive);
        }

        internal static SendContextData GetSendContextData(this SocketAsyncEventArgs args)
        {
            var sendContextData = args.UserToken as SendContextData;
            if(sendContextData == null)
            {
                throw new ArgumentException(nameof(args.UserToken));
            }
            return sendContextData;
        }
    }

    internal class ReceiveContextData : IDisposable
    {
        private ArraySegment<byte> m_ReceiveBuffer;

        internal ArraySegment<byte> ReceiveBuffer => this.m_ReceiveBuffer;

        internal ReceiveContextData(int initialBufferSize)
        {
            this.m_ReceiveBuffer = new ArraySegment<byte>(new byte[initialBufferSize], 0, 0);
        }

        public void Dispose()
        {
            this.m_ReceiveBuffer = default;
        }

        internal void ExpandReceiveBuffer(int bytesTransferred)
        {
            if (this.m_ReceiveBuffer.Count + bytesTransferred > this.m_ReceiveBuffer.Array.Length)
            {
                throw new ArgumentException(nameof(bytesTransferred));
            }
            this.m_ReceiveBuffer = new ArraySegment<byte>(this.m_ReceiveBuffer.Array,
                                                     this.m_ReceiveBuffer.Offset,
                                                     this.m_ReceiveBuffer.Count + bytesTransferred);
        }

        internal void SkipConsumedData(int bytesConsumed)
        {
            if (bytesConsumed > m_ReceiveBuffer.Count)
            {
                throw new ArgumentException(nameof(bytesConsumed));
            }

            m_ReceiveBuffer = new ArraySegment<byte>(m_ReceiveBuffer.Array, m_ReceiveBuffer.Offset + bytesConsumed, m_ReceiveBuffer.Count - bytesConsumed);
        }

        internal void CheckAndRearrangeReceiveBuffer(int bytesInNeed)
        {
            int capacity = this.m_ReceiveBuffer.Array.Length;
            int actualDataBytes = this.m_ReceiveBuffer.Count;
            int availableBeginning = this.m_ReceiveBuffer.Offset + actualDataBytes;
            int availableBytes = capacity - availableBeginning;

            if (availableBytes >= bytesInNeed)
            {
                return;
            }

            if (capacity - actualDataBytes >= bytesInNeed)
            {
                var originBuffer = this.m_ReceiveBuffer.Array;
                Buffer.BlockCopy(originBuffer, this.m_ReceiveBuffer.Offset, originBuffer, 0, actualDataBytes);
                this.m_ReceiveBuffer = new ArraySegment<byte>(originBuffer, 0, actualDataBytes);
            }
            else
            {
                int newCapacity = capacity * 2;
                while ((newCapacity - actualDataBytes) < bytesInNeed)
                {
                    newCapacity += newCapacity;
                }
                var newBuffer = new byte[newCapacity];
                var originBuffer = this.m_ReceiveBuffer.Array;
                Buffer.BlockCopy(originBuffer, this.m_ReceiveBuffer.Offset, newBuffer, 0, actualDataBytes);
                this.m_ReceiveBuffer = new ArraySegment<byte>(newBuffer, 0, actualDataBytes);
            }

            if (this.m_ReceiveBuffer.Array.Length - this.m_ReceiveBuffer.Offset + this.m_ReceiveBuffer.Count < bytesInNeed)
            {
                throw new Exception($"{nameof(ReceiveContextData)}.{nameof(CheckAndRearrangeReceiveBuffer)} 오류");
            }
        }
    }
}
