using System;
using System.Net;

namespace Client.Core
{
    public sealed class Client : IDisposable
    {
        private bool m_IsDisposed = false;
        private readonly IPAddress m_ServerIp;
        private readonly ClientConfig m_Config;

        public Client(ClientConfig config)
        {
            m_Config = config;
            m_ServerIp = IPAddress.Parse(config.ServerIPAddress);
        }

        public void Dispose()
        {
            if (m_IsDisposed)
            {
                return;
            }

            m_IsDisposed = true;
        }
    }
}
