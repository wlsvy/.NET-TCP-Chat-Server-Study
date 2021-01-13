using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Shared.Logger;
using Shared.Network;
using Shared.Util;

namespace ServerTest
{
    [TestClass]
    public class TcpTest
    {
        [TestMethod]
        public void ConnectionTest()
        {
            var csRecvQueues = new ConcurrentDictionary<long, BufferBlock<string>>();
            var scRecvQueues = new ConcurrentDictionary<long, BufferBlock<string>>();

            var csStreams = new ConcurrentDictionary<long, AsyncTCPConnection>();
            var scStreams = new ConcurrentDictionary<long, AsyncTCPConnection>();
        }

        [TestMethod]
        public void TestMethod1()
        {
            var ip = IPAddress.Any;
            var port = 10000;

            var logger = new ConsoleLogger();

            AsyncTcpConnection exceptionThrower = null;
            AsyncTcpConnection oppositeConnection = null;

            bool isOppositeTerminated = false;
            bool isExceptionTerminated = false;

            var acceptor = new AsyncTcpAcceptor(
                onNewConnection: (accepted) =>
                {
                    oppositeConnection = new AsyncTcpConnection(
                        socket: accepted, 
                        onSendError: exception => logger.Error("On Send Error", exception));
                    oppositeConnection.Subscribe(
                        onReceived: data => UnpackMessages(data, out var _),
                        onError: error => Assert.Fail("실패"),
                        onCompleted: () => isOppositeTerminated = true);

                },
                logger: logger);

            acceptor.Bind(ip, port);
            acceptor.ListenAndStart(32);

            var timeout = new[] { TimeSpan.FromMilliseconds(5000) };

            AsyncTcpConnector.Connect(ip, port, new Queue<TimeSpan>(timeout),
                (bool isConnected, Socket newSocket, object initialData) =>
                {
                    Assert.IsTrue(isConnected, "연결 실패");

                    exceptionThrower = new AsyncTcpConnection(newSocket);
                    exceptionThrower.Subscribe(
                        onReceived: data => throw new Exception("메세지 파싱"),
                        onError: error => isExceptionTerminated = true,
                        onCompleted: () => Assert.Fail("실패"));
                });

            TestHelper.BecomeTrue(() =>
            {
                return exceptionThrower != null 
                    && oppositeConnection != null;
            }, TimeSpan.FromMilliseconds(3000)).Wait();

            oppositeConnection.Send(new ArraySegment<byte>(Encoding.UTF8.GetBytes("핑퐁")));

            TestHelper.BecomeTrue(() =>
            {
                return (isExceptionTerminated == true) &&
                       (isOppositeTerminated);
            }, TimeSpan.FromSeconds(2)).Wait();

            Assert.IsTrue(isExceptionTerminated);
            Assert.IsTrue(isOppositeTerminated);

            acceptor.Dispose();

            exceptionThrower.Dispose();
            oppositeConnection.Dispose();
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
                    Assert.IsTrue(scStreams.TryAdd(scId, newConnection));

                    Action onColsed = () =>
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
                        onError: error => onColsed(),
                        onCompleted: onColsed);
                },
                logger: new ConsoleLogger());

            acceptor.Bind(ip, port);
            acceptor.ListenAndStart(32);

            for(int i = 0; i < numberOfConnections; i++)
            {
                var timeout = new Queue<TimeSpan>(new[] { TimeSpan.FromMilliseconds(5000) });
                AsyncTcpConnector.Connect(ip, port, timeout,
                    (bool isConnected, Socket socket, object initialData) =>
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
                                onCompleted: onClosed);
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
    }
}
