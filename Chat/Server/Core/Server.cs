using System.Diagnostics;
using System;
using System.Threading;
using System.Net;
using Server.Gui;
using Shared.Logger;

namespace Server.Core
{
    public sealed class Server : IDisposable
    {
        private readonly ServerConfig m_Config;
        private readonly Stopwatch m_Timer;
        private readonly ClientConnectionManager m_ClientConnectionManager;
        private readonly SessionManager m_SessionManager;

        private bool m_IsDisposed = false;

        public Server(ServerConfig config)
        {
            m_Config = config;
            m_Timer = new Stopwatch();
            m_ClientConnectionManager = new ClientConnectionManager(m_SessionManager);
            m_SessionManager = new SessionManager();
        }

        public void Start()
        {
            Log.I.Info("=========================\n \tServer Initialize.... \n=========================");

            try
            {
                var ipAddress = IPAddress.Parse(m_Config.CSListenIPAddress);

                m_ClientConnectionManager.Initialize(ipAddress, m_Config.CSListenPort, m_Config.NumberOfCSBacklogSockets);
            }
            catch (Exception e)
            {
                Log.I.Error($"서버 초기화 중 오류 발생", e);
            }
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
