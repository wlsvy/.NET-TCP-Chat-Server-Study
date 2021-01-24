using Shared.Network;
using System;
using System.Diagnostics;
using System.Net;
using System.Collections.Generic;
using Shared.Logger;
using System.Threading.Tasks;
using System.Threading;
using Shared.Gui;
using Client.Network;
using Shared.Protocol;
using System.Collections.Concurrent;

namespace Client
{
    public sealed class Client : IDisposable
    {
        private bool m_IsDisposed = false;
        private readonly ClientConfig m_Config;
        private readonly Stopwatch m_Timer = new Stopwatch();

        private ServerConnection m_ServerConnection;
        private readonly ConcurrentDictionary<PacketProtocol, TaskCompletionSource> m_UnresolvedRpcRequests = new ConcurrentDictionary<PacketProtocol, TaskCompletionSource>();

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
            veldridWindow.AddImguiRenderer(new ImguiDemoWindow());

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
            while (!cts.IsCancellationRequested)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1000));
            }
        }

        public async Task RequestCreateAccount(string id, string password)
        {
            var tcs = new TaskCompletionSource();
            if (!m_UnresolvedRpcRequests.TryAdd(PacketProtocol.CS_CreateAccount_REQ, tcs))
            {
                Log.I.Warn($"중복된 REQ 수행 시도, {nameof(RequestCreateAccount)}");
                return;
            }

            m_ServerConnection.PacketSender.SEND_CS_CreateAccount_REQ(id, password);
            //m_LinkedAccountId = await tcs.Task;

            if (!m_UnresolvedRpcRequests.TryRemove(PacketProtocol.CS_CreateAccount_REQ, out _))
            {
                Log.I.Warn($"REQ 정보 삭제 실패, 비정상 동작, {nameof(RequestCreateAccount)}");
            }
        }

        public void RequestLogin(string id, string password)
        {

        }
    }
}
