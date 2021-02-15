using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Shared.Util;
using Shared.Logger;
using System.Collections.Generic;
using System.Diagnostics;

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

        

        private const int INITIAL_READ_BYTES = 1024;
        private const int READ_BYTES_INCREASING_RATE = 2;

        public readonly IPEndPoint RemoteEndPoint;

        public bool IsConnected => m_Socket.Connected;
        public bool IsClosed => (Volatile.Read(ref m_IsDisposed) != 0);

        private readonly Socket m_Socket;
        private readonly SocketAsyncEventArgs m_SocketAsyncEventArgs = new SocketAsyncEventArgs { UserToken = new SendContextData() };
        private readonly Action<Exception> m_OnSendError;

        private Func<ArraySegment<byte>, int> m_OnReceived;
        private Action<Exception> m_OnReceiveError;
        private Action m_OnReceiveCompleted;

        private int m_IsDisposed = 0;

        private Operating m_Sending;
        private Operating m_Receiving;

        public AsyncTcpConnection(Socket socket) : this(socket, null) { }

        public AsyncTcpConnection(Socket socket, Action<Exception> onSendError)
        {
            m_Socket = socket ?? throw new ArgumentNullException(nameof(socket));
            m_OnSendError = onSendError;

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
            if(m_SocketAsyncEventArgs.UserToken is SendContextData sendContextData)
            {
                sendContextData.Dispose();
            }
        }

        public void Subscribe(Func<ArraySegment<byte>, int> onReceived, Action<Exception> onError, Action onReceiveCompleted)
        {
            m_OnReceived = onReceived ?? throw new ArgumentNullException(nameof(onReceived));
            m_OnReceiveError = onError ?? throw new ArgumentNullException(nameof(onError));
            m_OnReceiveCompleted = onReceiveCompleted ?? throw new ArgumentNullException(nameof(onReceiveCompleted));

            if (IsClosed)
            {
                m_OnReceiveError.Invoke(new SocketException((int)SocketError.OperationAborted));
            }
            else
            {
                TryReceive();
            }
        }

        private void TryReceive()
        {
            if (!m_Receiving.TryEnter())
            {
                return;
            }

            var receiveAsyncEventArgs = new SocketAsyncEventArgs
            {
                UserToken = new ReceiveContextData(2 * INITIAL_READ_BYTES)
            };
            receiveAsyncEventArgs.Completed += (sender, e) => OnReceiveCompleted(e);

            try
            {
                receiveAsyncEventArgs.PrepareReceiveBuffer(INITIAL_READ_BYTES);
                bool willRaiseEventLater;
                while(!(willRaiseEventLater = ReceiveAsync(receiveAsyncEventArgs)))
                {
                    var canReceive = CompleteReceive(receiveAsyncEventArgs);
                    if (!canReceive)
                    {
                        break;
                    }
                }
            }
            catch(Exception e)
            {
                CloseSocketWhileReceiving(receiveAsyncEventArgs, e);
            }
        }

        private void OnReceiveCompleted(SocketAsyncEventArgs args)
        {
            try
            {
                bool willRaiseEventLater;
                do
                {
                    var canReceive = CompleteReceive(args);
                    if (!canReceive)
                    {
                        break;
                    }
                } while (!(willRaiseEventLater = ReceiveAsync(args)));
            }
            catch(Exception e)
            {
                CloseSocketWhileReceiving(args, e);
            }
        }

        private bool ReceiveAsync(SocketAsyncEventArgs args)
        {
            if (IsClosed)
            {
                args.SocketError = SocketError.OperationAborted;
                return false;
            }
            return m_Socket.ReceiveAsync(args);
        }

        private bool CompleteReceive(SocketAsyncEventArgs args)
        {
            if(args.SocketError != SocketError.Success)
            {
                CloseSocketWhileReceiving(args, new SocketException((int)args.SocketError));
                return false;
            }

            var bytesReceiveRequested = args.Count;
            if(args.BytesTransferred <= 0 && 0 < bytesReceiveRequested)
            {
                CloseSocketWhileReceiving(args);
                return false;
            }

            var receivedEventData = args.UserToken as ReceiveContextData;
            receivedEventData.ExpandReceiveBuffer(args.BytesTransferred);

            var bytesConsumed = m_OnReceived(receivedEventData.ReceiveBuffer);
            if(bytesConsumed > 0)
            {
                receivedEventData.SkipConsumedData(bytesConsumed);
            }
            else if(bytesConsumed < 0)
            {
                return false;
            }

            int bytesToReceive;
            if(args.BytesTransferred < bytesReceiveRequested)
            {
                bytesToReceive = INITIAL_READ_BYTES;
            }
            else
            {
                var nextRequestReadBytes = READ_BYTES_INCREASING_RATE * bytesReceiveRequested;
                if(nextRequestReadBytes < 0)
                {
                    nextRequestReadBytes = int.MaxValue;
                }
                bytesToReceive = nextRequestReadBytes;
            }

            args.PrepareReceiveBuffer(bytesToReceive);
            return true;
        }

        private void CloseSocketWhileReceiving(SocketAsyncEventArgs args, Exception exception)
        {
            Log.I.Warn($"{nameof(AsyncTcpConnection)}.{nameof(CloseSocketWhileReceiving)} Close Socket! with error");

            m_Receiving.TryDispose();
            if(args.UserToken is ReceiveContextData receiveCtx)
            {
                receiveCtx.Dispose();
                args.UserToken = null;
            }
            args.Dispose();
            m_OnReceiveError.Invoke(exception);
            Dispose();
        }
        private void CloseSocketWhileReceiving(SocketAsyncEventArgs args)
        {
            Log.I.Info($"{nameof(AsyncTcpConnection)}.{nameof(CloseSocketWhileReceiving)} Close Socket!");

            m_Receiving.TryDispose();
            if (args.UserToken is ReceiveContextData receiveCtx)
            {
                receiveCtx.Dispose();
                args.UserToken = null;
            }
            args.Dispose();
            m_OnReceiveCompleted.Invoke();
            Dispose();
        }

        public void Send(ArraySegment<byte> sendBuffer)
        {
            if(sendBuffer.IsEmpty())
            {
                return;
            }

            m_SocketAsyncEventArgs.GetSendContextData().Add(sendBuffer);
            TrySend(m_SocketAsyncEventArgs);
        }
        public void Send(IList<ArraySegment<byte>> sendBuffer)
        {
            if (sendBuffer == null
                || sendBuffer.IsEmpty())
            {
                return;
            }

            m_SocketAsyncEventArgs.GetSendContextData().AddRange(sendBuffer);
            TrySend(m_SocketAsyncEventArgs);
        }

        public void TrySend(SocketAsyncEventArgs args)
        {
            if (!m_Sending.TryEnter())
            {
                return;
            }
            if (args.GetSendContextData().IsEmpty)
            {
                m_Sending.TryExit();
                return;
            }

            try
            {
                bool willRaiseEventLater;
                while(!(willRaiseEventLater = SendAsync(args)))
                {
                    var canSendAvailableBufferList = CompleteSend(args);
                    if (!canSendAvailableBufferList)
                    {
                        m_Sending.TryExit();
                        break;
                    }
                }
            }
            catch(Exception e)
            {
                CloseSocketWhileSending(args, e);
            }
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
                CloseSocketWhileSending(args, e);
            }
        }

        private bool SendAsync(SocketAsyncEventArgs args)
        {
            if (IsClosed)
            {
                args.SocketError = SocketError.OperationAborted;
                return false;
            }

            args.GetSendContextData().CopyBufferListTo(args);

            Debug.Assert(!args.BufferList.IsEmpty());

            return m_Socket.SendAsync(args);
        }

        private bool CompleteSend(SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
            {
                return false;
            }
            return args.GetSendContextData().Skip(args.BytesTransferred);
        }

        private void CloseSocketWhileSending(SocketAsyncEventArgs args, Exception exception)
        {
            if (m_Sending.TryDispose())
            {
                args.Dispose();
                if(args.UserToken is SendContextData sendContextData)
                {
                    sendContextData.Dispose();
                }
            }
            m_OnSendError?.Invoke(exception);
            Dispose();
        }
    }
}
