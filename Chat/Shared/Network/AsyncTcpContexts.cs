using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Shared.Util;

namespace Shared.Network
{
    internal sealed class SendContextData : IDisposable
    {
        private SpinLock m_LockSendingBufferQueue = new SpinLock(false);

        private bool m_IsDisposed = false;
        private readonly TcpSendBufferQueue m_SendBufferQueue = new TcpSendBufferQueue();

        public void Dispose()
        {
            var lockTaken = false;
            try
            {
                m_LockSendingBufferQueue.Enter(ref lockTaken);
                m_IsDisposed = true;
                m_SendBufferQueue.Clear();
                m_SendBufferQueue.TrimExcess();
            }
            finally
            {
                if (lockTaken)
                {
                    m_LockSendingBufferQueue.Exit();
                }
            }
        }

        internal bool IsEmpty
        {
            get
            {
                var lockTaken = false;
                try
                {
                    m_LockSendingBufferQueue.Enter(ref lockTaken);
                    return m_SendBufferQueue.List.IsEmpty();
                }
                finally
                {
                    if (lockTaken)
                    {
                        m_LockSendingBufferQueue.Exit();
                    }
                }
            }
        }

        internal void Add(ArraySegment<byte> sendBuffer)
        {
            var lockTaken = false;
            try
            {
                m_LockSendingBufferQueue.Enter(ref lockTaken);
                if (m_IsDisposed)
                {
                    return;
                }
                m_SendBufferQueue.Add(sendBuffer);
            }
            finally
            {
                if (lockTaken)
                {
                    m_LockSendingBufferQueue.Exit();
                }
            }
        }

        internal void AddRange(IEnumerable<ArraySegment<byte>> sendBuffers)
        {
            var lockTaken = false;
            try
            {
                m_LockSendingBufferQueue.Enter(ref lockTaken);
                if (m_IsDisposed)
                {
                    return;
                }
                foreach(var sendBuffer in sendBuffers)
                {
                    m_SendBufferQueue.Add(sendBuffer);
                }
            }
            finally
            {
                if (lockTaken)
                {
                    m_LockSendingBufferQueue.Exit();
                }
            }
        }

        internal void CopyBufferListTo(SocketAsyncEventArgs args)
        {
            var lockTaken = false;
            try
            {
                m_LockSendingBufferQueue.Enter(ref lockTaken);
                args.BufferList = m_SendBufferQueue.List;
            }
            finally
            {
                if (lockTaken)
                {
                    m_LockSendingBufferQueue.Exit(lockTaken);
                }
            }
        }

        internal bool Skip(int bytesSent)
        {
            var lockTaken = false;
            try
            {
                m_LockSendingBufferQueue.Enter(ref lockTaken);
                m_SendBufferQueue.TrimExcess();
                m_SendBufferQueue.Remove(bytesSent);
                return !m_SendBufferQueue.List.IsEmpty();
            }
            finally
            {
                if (lockTaken)
                {
                    m_LockSendingBufferQueue.Exit(lockTaken);
                }
            }
        }
    }
}
