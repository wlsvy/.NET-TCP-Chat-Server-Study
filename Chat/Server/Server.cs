using System.Diagnostics;
using System;

namespace Server
{
    public sealed class Server : IDisposable
    {
        private ServerGui m_ServerGui = new ServerGui();
        private Stopwatch m_Timer = new Stopwatch();
        private bool m_IsDisposed = false;

        public void Initialize()
        {
            m_ServerGui.Initialize();
            try
            {
                
            }
            catch(Exception e)
            {
                //TODO : Logger
            }
        }

        public void RunLoop(int timeSlicePerUpdate)
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
            }
        }

        public void Dispose()
        {
            if (m_IsDisposed)
            {
                return;
            }

            m_ServerGui.Dispose();
            m_IsDisposed = true;
        }
    }
}
