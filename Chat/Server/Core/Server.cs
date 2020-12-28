using System.Diagnostics;
using System;
using System.Threading;
using Server.Gui;
using Server.Logger;

namespace Server.Core
{
    public sealed class Server : IDisposable
    {
        private ServerGui m_ServerGui = new ServerGui();
        private Stopwatch m_Timer = new Stopwatch();

        private bool m_IsDisposed = false;

        public void Initialize()
        {
            InitializeSingleton();
            Log.I.Info("=========================\n \tServer Initialize.... \n=========================");

            try
            {
                m_ServerGui.Initialize();
            }
            catch (Exception e)
            {
                Log.I.Error($"서버 초기화 중 오류 발생 - [{e.Message}]");
            }
        }

        public void RunMainThreadLoop(int timeSlicePerUpdate)
        {
            m_Timer.Start();
            long elapsedTimeMSec = 0;

            while (true)
            {
                var currentElapsedTime = m_Timer.ElapsedMilliseconds;

                var deltaTimeMSec = currentElapsedTime - elapsedTimeMSec;
                elapsedTimeMSec = currentElapsedTime;

                if (m_ServerGui.IsWindowExist)
                {
                    m_ServerGui.Update((int)deltaTimeMSec);
                }
                else
                {
                    break;
                }

                var updateConsumedTime = m_Timer.ElapsedMilliseconds - elapsedTimeMSec;
                var sleepTime = timeSlicePerUpdate - updateConsumedTime;
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

            m_ServerGui.Dispose();

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
