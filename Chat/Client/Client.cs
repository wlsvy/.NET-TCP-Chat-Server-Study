using Client.Gui;
using Client.Network;
using Shared.Logger;
using Shared.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public sealed class Client : IDisposable
    {
        private bool m_IsDisposed = false;
        private readonly ClientConfig m_Config;
        private readonly Stopwatch m_Timer = new Stopwatch();

        private long m_LinkedAccountId = 0;
        public long LinkedAccountId => m_LinkedAccountId;

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
                onCompleted: (isSuccess, socket) =>
                {
                    isConnected = isSuccess;
                    if (!isConnected)
                    {
                        Log.I.Warn("서버 연결 실패");
                        waitToConnect.SetResult(false);
                        return;
                    }

                    ServerConnection.I.OnConnected(socket);
                    waitToConnect.SetResult(true);
                });

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
            var elapsedTimeMSec = 0L;

            timer.Start();
            ClientGui.I.VeldridWindow.Open("Simple Chat Client");
            ClientGui.I.VeldridWindow.AddImguiRenderer(new LoginWindow());

            while (true)
            {
                var currentElapsedTime = timer.ElapsedMilliseconds;
                var deltaTimeMSec = currentElapsedTime - elapsedTimeMSec;
                elapsedTimeMSec = currentElapsedTime;

                ClientJobManager.I.Update().Wait();

                if (ClientGui.I.VeldridWindow.IsWindowExist)
                {
                    ClientGui.I.VeldridWindow.Update((int)deltaTimeMSec);
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
            ClientGui.I.Destroy();
        }

        public void RunLoop(CancellationTokenSource cts)
        {
            while (!cts.IsCancellationRequested)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1000));
            }
        }
    }
}
