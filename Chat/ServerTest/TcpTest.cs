using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Shared.Logger;
using Shared.Network;
using Shared.Util;

[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]

namespace ServerTest
{
    [TestClass]
    public class TcpTest
    {
        [TestMethod]
        public async Task ConnectionTest()
        {
            var csRecvQueues = new ConcurrentDictionary<long, BufferBlock<string>>();
            var scRecvQueues = new ConcurrentDictionary<long, BufferBlock<string>>();

            var csStreams = new ConcurrentDictionary<long, AsyncTcpConnection>();
            var scStreams = new ConcurrentDictionary<long, AsyncTcpConnection>();

            var numberOfConnection = 3;

            await PrepareSendRecvTcpStream(IPAddress.Loopback, 2022, numberOfConnection, csStreams, scStreams, csRecvQueues, scRecvQueues);

            int numberOfMessages = 100;
            var serverReceivedMessages = scRecvQueues.First().Value;
            var clientReceivedMessages = csRecvQueues.First().Value;
            var client = csStreams.First().Value;
            var server = scStreams.First().Value;

            var csTask = Task.Run(async () =>
            {
                //c -> S
                for (int i = 0; i < numberOfMessages; ++i)
                {
                    client.Send(PackMessage($"CS Message #{i}"));
                    if (i == numberOfMessages / 3)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(1));
                    }
                }

                // S->C 
                for (int i = 0; i < numberOfMessages; ++i)
                {
                    await clientReceivedMessages.Expect($"SC Message #{i}", TimeSpan.FromSeconds(5));
                    Log.I.Info($"SC Message #{i}");
                    if (i == numberOfMessages / 2)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(1));
                    }
                }

                client.Send(PackMessage("CSFINISH"));
                await clientReceivedMessages.Expect("SCFINISH", TimeSpan.FromSeconds(5));
            });

            var scTask = Task.Run(async () =>
            {
                // S->C 
                for (int i = 0; i < numberOfMessages; ++i)
                {
                    server.Send(PackMessage($"SC Message #{i}"));
                    if (i == numberOfMessages / 3)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(1));
                    }
                }

                // C->S ���� Ȯ�� 
                for (int i = 0; i < numberOfMessages; ++i)
                {
                    await serverReceivedMessages.Expect($"CS Message #{i}", TimeSpan.FromSeconds(5));
                    Log.I.Info($"CS Message #{i}");
                    if (i == numberOfMessages / 2)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(1));
                    }
                }

                server.Send(PackMessage("SCFINISH"));
                await serverReceivedMessages.Expect("CSFINISH", TimeSpan.FromSeconds(5));
            });

            Task.WaitAll(scTask, csTask);

            Assert.AreEqual(csStreams.Count, numberOfConnection);
            Assert.AreEqual(scStreams.Count, numberOfConnection);

            Assert.AreEqual(serverReceivedMessages.Count, 0);
            Assert.AreEqual(clientReceivedMessages.Count, 0);

            foreach(var item in csStreams.Values)
            {
                item.Dispose();
            }
            foreach (var item in scStreams.Values)
            {
                item.Dispose();
            }

            var beginTime = DateTime.UtcNow;
            while ((csStreams.Count > 0) || (scStreams.Count > 0))
            {
                await Task.Delay(100);
                Assert.IsTrue((DateTime.UtcNow - beginTime) < TimeSpan.FromMilliseconds(1000));
            }
        }

        [TestMethod]
        public async Task ExceptionFromApplication()
        {
            // ���ø����̼� �������� onReceiveTCPData()�� ó���ϸ鼭 ���ܰ� �߻��ϴ� ���,
            // ������ ������� onTerminated�� �ҷ��� �Ѵ�. (����Ŀ�ؼ����� ���� �ʵ���)

            var ip = IPAddress.Loopback;
            var port = 10000;

            AsyncTcpConnection clientSideConnection = null;
            AsyncTcpConnection serverSideConnection = null;

            bool isOppositeTerminated = false;
            bool isExceptionTerminated = false;

            var acceptor = new AsyncTcpAcceptor(
                onNewConnection: (accepted) =>
                {
                    serverSideConnection = new AsyncTcpConnection(
                        socket: accepted, 
                        onSendError: exception => Log.I.Error("On Send Error", exception));
                    serverSideConnection.Subscribe(
                        onReceived: data => UnpackMessages(data, out var _),
                        onError: error => Assert.Fail("����"),
                        onReceiveCompleted: () => isOppositeTerminated = true);
                });

            acceptor.Bind(ip, port);
            acceptor.ListenAndStart(1);

            var timeout = new[] { TimeSpan.FromMilliseconds(5000), TimeSpan.FromMilliseconds(7000) };

            AsyncTcpConnector.Connect(
                    ip: ip,
                    port: port,
                    leftTimeoutList: new Queue<TimeSpan>(timeout),
                    onCompleted: (bool isConnected, Socket newSocket) =>
                    {
                        Assert.IsTrue(isConnected, "���� ����");

                        clientSideConnection = new AsyncTcpConnection(newSocket);
                        clientSideConnection.Subscribe(
                            onReceived: data => throw new Exception("�޼��� �Ľ�"),
                            onError: error => isExceptionTerminated = true,
                            onReceiveCompleted: () => Assert.Fail("����"));   //���������� �޼��� ���� �� �׽�Ʈ ����
                    });

            await TestHelper.BecomeTrue(() =>
            {
                return clientSideConnection != null
                    && serverSideConnection != null;
            }, TimeSpan.FromMilliseconds(3000));

            serverSideConnection.Send(new ArraySegment<byte>(Encoding.UTF8.GetBytes("����")));

            await TestHelper.BecomeTrue(() =>
            {
                return isExceptionTerminated && isOppositeTerminated;
            }, TimeSpan.FromSeconds(2));

            Assert.IsTrue(isExceptionTerminated);
            Assert.IsTrue(isOppositeTerminated);

            acceptor.Dispose();

            clientSideConnection.Dispose();
            serverSideConnection.Dispose();
        }

        [TestMethod]
        public void DuplicatedPortBinding()
        {
            var first = new AsyncTcpAcceptor(
                onNewConnection: accepted =>
                {
                    //Do nothing
                    accepted.Close();
                });
            var second = new AsyncTcpAcceptor(
                onNewConnection: accepted =>
                {
                    //Do nothing
                    accepted.Close();
                });

            first.Bind(IPAddress.Loopback, 12254);
            Assert.ThrowsException<SocketException>(() =>
            {
                second.Bind(IPAddress.Loopback, 12254);
            });

            Assert.ThrowsException<SocketException>(() =>
            {
                first.Bind(IPAddress.Loopback, 12255);
            });

            second.Bind(IPAddress.Loopback, 12256);

            first.ListenAndStart(4);
            second.ListenAndStart(4);

            Assert.IsTrue(Uri.TryCreate($"tcp://{first.LocalEndpoint.ToString()}", UriKind.Absolute, out var firstListenUri));
            Assert.IsTrue(firstListenUri.Port == 12254);

            Assert.IsTrue(Uri.TryCreate($"tcp://{second.LocalEndpoint.ToString()}", UriKind.Absolute, out var secondListenUri));
            Assert.IsTrue(secondListenUri.Port == 12256);
        }

        [TestMethod]
        public void AsyncTcpAcceptor()
        {
            var listenIp = IPAddress.Loopback;
            int listenPort = 12255;

            var acceptedConnections = new ConcurrentQueue<AsyncTcpConnection>();

            var acceptor = new AsyncTcpAcceptor(
                onNewConnection: accepted =>
                {
                    var newConnection = new AsyncTcpConnection(accepted);
                    newConnection.Subscribe(_ => 0, _ => { }, () => { });
                    acceptedConnections.Enqueue(newConnection);
                });

            int numberOfBacklogSockets = 8;
            acceptor.Bind(listenIp, listenPort);
            acceptor.ListenAndStart(numberOfBacklogSockets);

            var isDone = new AutoResetEvent(false);

            Func<Socket, AsyncTcpConnection> tcpStreamCreator = socket =>
            {
                var newStream = new AsyncTcpConnection(socket);
                newStream.Subscribe(_ => 0, _ => { }, () => { });
                return newStream;
            };

            AsyncTcpConnector.Connect(
                ip: listenIp,
                port: listenPort,
                leftTimeoutList: new Queue<TimeSpan>(new[]
                {
                    TimeSpan.FromMilliseconds(100),
                    TimeSpan.FromMilliseconds(100),
                    TimeSpan.FromMilliseconds(100),
                    TimeSpan.FromMilliseconds(100) }),
                onCompleted: (isConnected, socket) =>
                {
                    Assert.IsTrue(isConnected);
                    Assert.IsNotNull(socket);

                    var newStream = tcpStreamCreator(socket);
                    Assert.IsNotNull(newStream);

                    isDone.Set();
                });

            Assert.IsTrue(isDone.WaitOne(TimeSpan.FromSeconds(5)));
            isDone.Reset();

            BecomeTrue(()=>
            {
                return (acceptedConnections.Count >= 1);
            }, TimeSpan.FromSeconds(1)).Wait();

            var connectionCount = 5 * numberOfBacklogSockets;

            for(int i = 0; i < connectionCount; i++){
                AsyncTcpConnector.Connect(
                ip: listenIp,
                port: listenPort,
                leftTimeoutList: new Queue<TimeSpan>(new[]
                {
                    TimeSpan.FromMilliseconds(100),
                    TimeSpan.FromMilliseconds(100),
                    TimeSpan.FromMilliseconds(100),
                    TimeSpan.FromMilliseconds(100) }),
                onCompleted: (isConnected, socket) =>
                {
                    Assert.IsTrue(isConnected);
                    Assert.IsNotNull(socket);

                    var newStream = tcpStreamCreator(socket);
                    Assert.IsNotNull(newStream);

                    isDone.Set();
                });
            }

            BecomeTrue(() =>
            {
                return (acceptedConnections.Count >= connectionCount);
            }, TimeSpan.FromSeconds(1)).Wait();

            foreach(var c in acceptedConnections)
            {
                c.Dispose();
            }
        }

        private static async Task PrepareSendRecvTcpStream(
            IPAddress ip,
            int port,
            int numberOfConnections,
            ConcurrentDictionary<long, AsyncTcpConnection> csStreams,
            ConcurrentDictionary<long, AsyncTcpConnection> scStreams,
            ConcurrentDictionary<long, BufferBlock<string>> csRecvQueues,
            ConcurrentDictionary<long, BufferBlock<string>> scRecvQueues)
        {
            for(long i = 0; i < numberOfConnections; i++)
            {
                csRecvQueues.TryAdd(i, new BufferBlock<string>(new DataflowBlockOptions { BoundedCapacity = DataflowBlockOptions.Unbounded}));
                scRecvQueues.TryAdd(i, new BufferBlock<string>(new DataflowBlockOptions { BoundedCapacity = DataflowBlockOptions.Unbounded}));
            }

            var csIds = new IdGenerator(0);
            var scIds = new IdGenerator(0);

            var acceptor = new AsyncTcpAcceptor(
                onNewConnection: (socket) =>
                {
                    long scId = scIds.Generate();
                    var newConnection = new AsyncTcpConnection(socket);
                    scStreams.TryAdd(scId, newConnection);

                    Action onClosed = () =>
                    {
                        scRecvQueues.TryGetValue(scId, out var scRecvQueue);
                        Assert.AreEqual(scRecvQueue.Count, 0);
                        Assert.IsTrue(scStreams.TryRemove(scId, out var temp));
                    };

                    newConnection.Subscribe(
                        onReceived: tcpData =>
                        {
                            int parsedBytes = UnpackMessages(tcpData, out var messages);
                            scRecvQueues.TryGetValue(scId, out var scRecvQueue);
                            foreach (var message in messages)
                            {
                                scRecvQueue.Post(message);
                            }
                            return parsedBytes;
                        },
                        onError: error => Assert.Fail("AsyncTcpAcceptor ����"),
                        onReceiveCompleted: onClosed);
                });

            acceptor.Bind(ip, port);
            acceptor.ListenAndStart(5);

            for(int i = 0; i < numberOfConnections; i++)
            {
                var timeout = new Queue<TimeSpan>(new[] { TimeSpan.FromMilliseconds(5000) });
                AsyncTcpConnector.Connect(
                    ip: ip, 
                    port: port, 
                    leftTimeoutList: timeout,
                    onCompleted: (bool isConnected, Socket socket) =>
                    {
                        Assert.IsTrue(isConnected);

                        var csId = csIds.Generate();
                        var newConnection = new AsyncTcpConnection(socket);
                        Assert.IsTrue(csStreams.TryAdd(csId, newConnection));

                        Action onClosed = () =>
                        {
                            csRecvQueues.TryGetValue(csId, out var csRecvQueue);
                            Assert.AreEqual(csRecvQueue.Count, 0);
                            Assert.IsTrue(csStreams.TryRemove(csId, out var temp));
                        };

                        newConnection.Subscribe(
                                onReceived: tcpData =>
                                {
                                    int parsedBytes = UnpackMessages(tcpData, out var messages);
                                    csRecvQueues.TryGetValue(csId, out var csRecvQueue);
                                    foreach (var message in messages)
                                    {
                                        csRecvQueue.Post(message);
                                    }
                                    return parsedBytes;
                                },
                                onError: error => onClosed(),
                                onReceiveCompleted: onClosed);
                    });
            }

            while (csStreams.Count() < numberOfConnections ||
                    scStreams.Count() < numberOfConnections)
            {
                await Task.Delay(0);
            }

            acceptor.Dispose();
        }

        public static int UnpackMessages(ArraySegment<byte> tcpData, out IList<string> messages)
        {
            var leftData = tcpData;
            int totalParsedBytes = 0;

            messages = new List<string>();

            int parsedBytes = UnpackMessage(leftData, out string message);
            while (parsedBytes > 0)
            {
                totalParsedBytes += parsedBytes;
                messages.Add(message);

                leftData = new ArraySegment<byte>(leftData.Array, leftData.Offset + parsedBytes,
                    leftData.Count - parsedBytes);
                parsedBytes = UnpackMessage(leftData, out message);
            }

            return totalParsedBytes;
        }

        private static int UnpackMessage(ArraySegment<byte> tcpData, out string message)
        {
            if (tcpData.Count < 4)
            {
                message = null;
                return 0;
            }

            using (var ms = new MemoryStream(tcpData.Array, tcpData.Offset, tcpData.Count))
            using (var reader = new BinaryReader(ms))
            {
                long startPosition = ms.Position;

                int length = reader.ReadInt32();
                Assert.IsTrue(length > 0);

                if (tcpData.Count < length * sizeof(byte) + sizeof(int))
                {
                    message = null;
                    return 0;
                }

                message = Encoding.UTF8.GetString(reader.ReadBytes(length));
                return (int)(ms.Position - startPosition);
            }
        }

        public static ArraySegment<byte> PackMessage(string message)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                long startPosition = ms.Position;
                byte[] binaryMessage = Encoding.UTF8.GetBytes(message);
                int length = binaryMessage.Length;
                Assert.IsTrue(length > 0);
                writer.Write(length);
                writer.Write(binaryMessage);
                writer.Flush();

                return new ArraySegment<byte>(ms.GetBuffer(), (int)startPosition, (int)(ms.Position - startPosition));
            }
        }

        public static Task BecomeTrue(Func<bool> condition, TimeSpan timeout)
        {
            return BecomeTrue(condition, timeout, string.Empty);
        }

        public static async Task BecomeTrue(Func<bool> condition, TimeSpan timeout, string message)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            var until = DateTime.UtcNow + timeout;
            while (!condition())
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1));
                if (until <= DateTime.UtcNow)
                {
                    Assert.Fail(message);
                }
            }
        }
    }
}
