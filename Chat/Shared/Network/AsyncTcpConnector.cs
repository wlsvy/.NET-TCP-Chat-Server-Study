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
        public static void Connect(IPAddress ip, int port, Queue<TimeSpan> leftTimeoutList, Action<bool, Socket, object> onCompleted)
        {
            if (ip == null)
            {
                throw new ArgumentNullException(nameof(ip));
            }

            var remoteEndPoint = new IPEndPoint(ip, port);

            DoTryConnect(remoteEndPoint, leftTimeoutList, onCompleted);
        }

        private static void DoTryConnect(EndPoint remoteEndPoint, Queue<TimeSpan> leftTimeoutList, Action<bool, Socket, object> onCompleted)
        {
            if (remoteEndPoint == null)
            {
                throw new ArgumentNullException(nameof(remoteEndPoint));
            }

            Log.I.Info("접속 시도...");

            if (leftTimeoutList.IsEmpty())
            {
                Log.I.Info("접속 최종 실패");
                onCompleted(false, null, null);
                return;
            }

            var nextTimeout = leftTimeoutList.Dequeue();

            var socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            var args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = remoteEndPoint;
            args.Completed += OnConnect;
            //var connectContext = new ConnectionContext(connectSocket, leftTimeoutList, onCompleted)
                
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

                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"서버 연결 실패 - [{e.Message}]");
                DisposeConnectionArgs(args);
            }
        }

        private static void OnConnect(object sender, SocketAsyncEventArgs args)
        {
            ProcessConnect(args);
        }

        private static void ProcessConnect(SocketAsyncEventArgs args)
        {

        }

        private static void DisposeConnectionArgs(SocketAsyncEventArgs args)
        {
            args.Dispose();
        }

        private class ConnectionContext : IDisposable
        {
            public readonly Socket ConnectedSocket;
            public readonly Action<bool, Socket, object> OnCompleted;
            public readonly Queue<TimeSpan> LeftTimeoutList;
            public readonly object InitialData;

            private Task m_ConnectTimeoutTask;
            private readonly CancellationTokenSource m_CancelToken = new CancellationTokenSource();
            private readonly object m_Lock = new object();
            private bool m_IsCompleted;
            private bool m_IsDispoed;

            public ConnectionContext(Socket socket, Queue<TimeSpan> leftTimeoutList, Action<bool, Socket, object> onCompleted, object initialData)
            {
                _ = socket ?? throw new ArgumentNullException(nameof(socket));
                _ = leftTimeoutList ?? throw new ArgumentNullException(nameof(leftTimeoutList));
                _ = onCompleted ?? throw new ArgumentNullException(nameof(onCompleted));

                ConnectedSocket = socket;
                OnCompleted = onCompleted;
                LeftTimeoutList = leftTimeoutList;
                InitialData = initialData;
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

            public void RegisterTimeout(TimeSpan timeout, Action<ConnectionContext> timeoutCallback)
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
                    }, m_CancelToken.Token)
                }
                catch (Exception e)
                {
                    Log.I.Error($"{nameof(ConnectionContext)}.{nameof(RegisterTimeout)} 타임아웃 처리 중 오류 발생", e);
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
