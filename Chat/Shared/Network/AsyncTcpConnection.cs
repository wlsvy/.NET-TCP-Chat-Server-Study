using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Shared.Network
{
    public class AsyncTcpConnection : IDisposable
    {
        private struct Operating
        {
            private const int Free = 0;
            private const int InProgress = 1;
            private const int Disposed = 2;

            private int value;

            public bool TryEnter()
            {
                return Interlocked.CompareExchange(ref value, InProgress, Free) == Free;
            }
            public bool TryExit()
            {
                var original = Interlocked.CompareExchange(ref value, Free, InProgress);
                if (original == Free)
                {
                    throw new Exception($"{nameof(AsyncTcpConnection)}.{nameof(TryExit)}");
                }
                return original == InProgress;
            }
            public bool TryDispose()
            {
                return Interlocked.Exchange(ref value, Disposed) != Disposed;
            }
        }

        public readonly IPEndPoint RemoteEndPoint;

        public bool IsConnected => m_Socket.Connected;
        public bool IsClosed => (Volatile.Read(ref m_IsDisposed) != 0);

        private readonly Socket m_Socket;
        private readonly SocketAsyncEventArgs m_SocketAsyncEventArgs = new SocketAsyncEventArgs();

        private int m_IsDisposed = 0;

        private Operating m_Sending;
        private Operating m_Receiving;


        public AsyncTcpConnection(Socket socket)
        {
            m_Socket = socket ?? throw new ArgumentNullException(nameof(socket));

            if (m_Socket.RemoteEndPoint is IPEndPoint remoteEndPoint)
            {
                RemoteEndPoint = remoteEndPoint;
            }
            else
            {
                throw new ArgumentException(nameof(socket));
            }
            m_SocketAsyncEventArgs.Completed += (sender, e) => OnSendCompleted(e);
        }

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref m_IsDisposed, 1, 0) != 0)
            {
                return;
            }

            m_Socket.Shutdown(SocketShutdown.Both);
            m_Socket.Close(timeout: 0);
            m_SocketAsyncEventArgs.Dispose();
        }

        private void OnSendCompleted(SocketAsyncEventArgs args)
        {
            try
            {
                bool willRaiseEventLater;
                do
                {
                    var canSendAvailableBufferList = CompleteSend(args);
                    if (!canSendAvailableBufferList)
                    {
                        break;
                    }
                    willRaiseEventLater = SendAsync(args);
                } while (!willRaiseEventLater);
            }
            catch (Exception e)
            {

            }
        }

        private bool SendAsync(SocketAsyncEventArgs args)
        {
            if (IsClosed)
            {
                args.SocketError = SocketError.OperationAborted;
                return false;
            }
            //....//
            return m_Socket.SendAsync(args);
        }

        private bool CompleteSend(SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
            {
                return false;
            }
            return true;
            //return args.GetSend
        }

        private void CloseSocketWhileSending(SocketAsyncEventArgs args, Exception exception)
        {
            if (m_Sending.TryDispose())
            {
                args.Dispose();
            }
            Dispose();
        }
    }
}
