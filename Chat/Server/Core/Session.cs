using System;

namespace Server.Core
{
    public sealed class Session : IDisposable
    {
        public readonly long SessionId;

        private bool m_IsDisposed = false;

        public Session(long id)
        {
            SessionId = id;
        }

        public void Dispose()
        {
            if (m_IsDisposed)
            {
                return;
            }
            m_IsDisposed = true;
        }

        public void OnDisconnected()
        {

        }
    }
}
