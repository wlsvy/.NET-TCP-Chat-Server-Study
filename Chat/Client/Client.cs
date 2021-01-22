using Shared.Network;
using System;
using System.Diagnostics;
using System.Net;
using System.Collections.Generic;
using Shared.Logger;
using System.Threading.Tasks;
using System.Threading;
using Shared.Gui;

namespace Client
{
    public sealed class Client : IDisposable
    {
        private bool m_IsDisposed = false;
        private readonly ClientConfig m_Config;
        private readonly Stopwatch m_Timer = new Stopwatch();

        private ServerConnection m_ServerConnection;

        public Client(ClientConfig config)
        {
            m_Config = config;
        }

        public bool TryConnectToServer()
        {
            var serverIp = IPAddress.Parse(m_Config.ServerIPAddress);
            var waitToConnect = new TaskCompletionSource<bool>();
            bool isConnected = false;

            AsyncTcpConnector.Connect(
                ip: serverIp,
                port: m_Config.ServerPort,
                leftTimeoutList: new Queue<TimeSpan>(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                }),
                onCompleted: (isSuccess, socket, initialiData) =>
                {
                    isConnected = isSuccess;
                    if (!isConnected)
                    {
                        Log.I.Warn("서버 연결 실패");
                        waitToConnect.SetResult(false);
                        return;
                    }

                    m_ServerConnection = new ServerConnection(socket);
                    waitToConnect.SetResult(true);
                },
                initialData: null);

            waitToConnect.Task.Wait();
            return isConnected;
        }

        public void Dispose()
        {
            if (m_IsDisposed)
            {
                return;
            }

            m_IsDisposed = true;
        }

        public void RunLoop()
        {
            var timeSlicePerUpdateMSec = m_Config.TimeSlicePerUpdateMSec;
            var timer = new Stopwatch();
            var veldridWindow = new VeldridWindow();
            var elapsedTimeMSec = 0L;

            timer.Start();
            veldridWindow.Open();

            while (true)
            {
                var currentElapsedTime = timer.ElapsedMilliseconds;
                var deltaTimeMSec = currentElapsedTime - elapsedTimeMSec;
                elapsedTimeMSec = currentElapsedTime;

                if (veldridWindow.IsWindowExist)
                {
                    veldridWindow.Update((int)deltaTimeMSec);
                }
                else
                {
                    break;
                }

                var updateConsumedTime = timer.ElapsedMilliseconds - elapsedTimeMSec;
                var sleepTime = timeSlicePerUpdateMSec - updateConsumedTime;
                if (sleepTime > 0)
                {
                    Thread.Sleep((int)sleepTime);
                }
            }
            veldridWindow.Dispose();
        }

        public void RunLoop(CancellationTokenSource cts)
        {
            var timeSlicePerUpdateMSec = m_Config.TimeSlicePerUpdateMSec;
            while (!cts.IsCancellationRequested)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1000));
            }
        }
    }
}
