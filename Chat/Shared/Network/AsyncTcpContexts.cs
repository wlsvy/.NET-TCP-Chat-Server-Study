using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Shared.Network
{
    internal sealed class SendContextData : IDisposable
    {
        private SpinLock m_LockSendingBufferQueue = new SpinLock(false);

        private bool m_IsDisposed = false;

        public void Dispose()
        {
            var lockTaken = false;
            try
            {
                m_LockSendingBufferQueue.Enter(ref lockTaken);
                m_IsDisposed = true;
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
}
