using Shared.Logger;
using Shared.Util;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Network
{
    public static class AsyncTcpConnector
    {
        public static void Connect(IPAddress ip, int port, Queue<TimeSpan> leftTimeoutList, Action<bool, Socket> onCompleted)
        {
            _ = ip ?? throw new ArgumentNullException(nameof(ip));

            var remoteEndPoint = new IPEndPoint(ip, port);
            DoTryConnect(remoteEndPoint, leftTimeoutList, onCompleted);
        }

        private static void DoTryConnect(EndPoint remoteEndPoint, Queue<TimeSpan> leftTimeoutList, Action<bool, Socket> onCompleted)
        {
            _ = remoteEndPoint ?? throw new ArgumentNullException(nameof(remoteEndPoint));
            _ = leftTimeoutList ?? throw new ArgumentNullException(nameof(leftTimeoutList));
            _ = onCompleted ?? throw new ArgumentNullException(nameof(onCompleted));

            if (leftTimeoutList.IsEmpty())
            {
                Log.I.Info("접속 최종 실패");
                onCompleted(false, null);
                return;
            }

            Log.I.Info("접속 시도...");
            var nextTimeout = leftTimeoutList.Dequeue();

            var socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            var connectContextData = new ConnectionContextData(socket, leftTimeoutList, onCompleted);
            var args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = remoteEndPoint;
            args.Completed += OnConnect;
            args.UserToken = connectContextData;
                
            try
            {
                bool willRaiseEventLater = socket.ConnectAsync(args);
                if (!willRaiseEventLater)
                {
                    //연결이 바로 완료될 시
                    ProcessConnect(args);
                }
                else
                {
                    connectContextData.RegisterTimeout(
                        timeout: nextTimeout,
                        timeoutCallback: data =>
                    {
                        data.ConnectedSocket.Close();
                        Log.I.Info($"타임아웃으로 접속 실패. 잠시 후 접속 재시도");

                        DoTryConnect(remoteEndPoint, data.LeftTimeoutList, data.OnCompleted);
                    });
                }
            }
            catch (Exception e)
            {
                Log.I.Error($"{nameof(AsyncTcpConnector)}.{nameof(DoTryConnect)} 접속 중 오류 발생, 잠시 후 접속 재시도", e);
                DoTryConnect(remoteEndPoint, leftTimeoutList, onCompleted);
                DisposeConnectionArgs(args);
            }
        }

        private static void OnConnect(object sender, SocketAsyncEventArgs args)
        {
            var connectContextData = args.UserToken as ConnectionContextData;

            ProcessConnect(args);
        }

        private static void ProcessConnect(SocketAsyncEventArgs args)
        {
            var connectContextData = args.UserToken as ConnectionContextData;
            if (!connectContextData.CancelTimeout())
            {
                connectContextData.ConnectedSocket.Close();
                DisposeConnectionArgs(args);
                return;
            }

            if(args.SocketError != SocketError.Success)
            {
                Log.I.Warn($"접속 실패");
                DoTryConnect(args.RemoteEndPoint, connectContextData.LeftTimeoutList, connectContextData.OnCompleted);
                DisposeConnectionArgs(args);
                return;
            }

            Log.I.Info("TCP 접속 성공");
            connectContextData.OnCompleted(true, connectContextData.ConnectedSocket);
            DisposeConnectionArgs(args);
        }

        private static void DisposeConnectionArgs(SocketAsyncEventArgs args)
        {
            var connectContextData = args.UserToken as ConnectionContextData;
            connectContextData?.Dispose();
            args.Dispose();
        }

        private class ConnectionContextData : IDisposable
        {
            public readonly Socket ConnectedSocket;
            public readonly Action<bool, Socket> OnCompleted;
            public readonly Queue<TimeSpan> LeftTimeoutList;

            private Task m_ConnectTimeoutTask;
            private readonly CancellationTokenSource m_CancelToken = new CancellationTokenSource();
            private readonly object m_Lock = new object();
            private bool m_IsCompleted;
            private bool m_IsDispoed;

            public ConnectionContextData(Socket socket, Queue<TimeSpan> leftTimeoutList, Action<bool, Socket> onCompleted)
            {
                ConnectedSocket = socket ?? throw new ArgumentNullException(nameof(socket));
                OnCompleted = onCompleted ?? throw new ArgumentNullException(nameof(onCompleted));
                LeftTimeoutList = leftTimeoutList ?? throw new ArgumentNullException(nameof(leftTimeoutList));
            }

            public void Dispose() 
            {
                if (m_IsDispoed)
                {
                    return;
                }
                m_IsDispoed = true;

                m_CancelToken.Cancel();
                m_CancelToken.Dispose();
                m_ConnectTimeoutTask = null;
            }

            public void RegisterTimeout(TimeSpan timeout, Action<ConnectionContextData> timeoutCallback)
            {
                _ = timeoutCallback ?? throw new ArgumentNullException(nameof(timeoutCallback));

                if (m_CancelToken.IsCancellationRequested)
                {
                    return;
                }

                try
                {
                    m_ConnectTimeoutTask = Task.Delay(timeout, m_CancelToken.Token).ContinueWith((task) =>
                    {
                        if (!TrySetCompleted())
                        {
                            return;
                        }
                        timeoutCallback(this);
                    }, m_CancelToken.Token);
                }
                catch (Exception e)
                {
                    Log.I.Error($"{nameof(ConnectionContextData)}.{nameof(RegisterTimeout)} 타임아웃 처리 중 오류 발생", e);
                }

            }

            public bool CancelTimeout() 
            {
                if (!TrySetCompleted())
                {
                    return false;
                }
                m_CancelToken.Cancel();
                return true;
            }

            private bool TrySetCompleted()
            {
                lock (m_Lock)
                {
                    if (m_IsCompleted)
                    {
                        return false;
                    }

                    m_IsCompleted = true;
                    return true;
                }
            }
        }
    }
}
