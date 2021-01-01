using System.Diagnostics;
using System;
using System.Threading;
using Server.Gui;
using Shared.Logger;

namespace Server.Core
{
    public sealed class Server : IDisposable
    {
        private readonly ServerConfig m_Config;
        private VeldridWindow m_GuiWindow = new VeldridWindow();
        private Stopwatch m_Timer = new Stopwatch();

        private bool m_IsDisposed = false;

        public Server(ServerConfig config)
        {
            m_Config = config;
        }

        public void Initialize()
        {
            InitializeSingleton();
            Log.I.Info("=========================\n \tServer Initialize.... \n=========================");

            try
            {
                m_GuiWindow.Open();
            }
            catch (Exception e)
            {
                Log.I.Error($"서버 초기화 중 오류 발생 - [{e.Message}]");
            }
        }

        public void RunMainThreadLoop()
        {
            m_Timer.Start();
            long elapsedTimeMSec = 0;

            while (true)
            {
                var currentElapsedTime = m_Timer.ElapsedMilliseconds;

                var deltaTimeMSec = currentElapsedTime - elapsedTimeMSec;
                elapsedTimeMSec = currentElapsedTime;

                if (m_GuiWindow.IsWindowExist)
                {
                    m_GuiWindow.Update((int)deltaTimeMSec);
                }
                else
                {
                    break;
                }

                var updateConsumedTime = m_Timer.ElapsedMilliseconds - elapsedTimeMSec;
                var sleepTime = m_Config.TimeSlicePerUpdateMSec - updateConsumedTime;
                if (sleepTime > 0)
                {
                    Thread.Sleep((int)sleepTime);
                }
            }
        }

        public void Dispose()
        {
            if (m_IsDisposed)
            {
                return;
            }

            m_GuiWindow.Dispose();

            DestroySingleton();

            m_IsDisposed = true;
        }

        private void InitializeSingleton()
        {
            Log.I.Initialize();
        }

        private void DestroySingleton()
        {
            Log.I.Destroy();
        }
    }
}
