using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Shared.Logger;
using Shared.Network;

namespace ServerTest
{
    [TestClass]
    public class TcpTest
    {
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
                        onError: error => Assert.Fail("����"),
                        onCompleted: () => isOppositeTerminated = true);

                },
                logger: logger);

            acceptor.Bind(ip, port);
            acceptor.ListenAndStart(32);

            var timeout = new[] { TimeSpan.FromMilliseconds(5000) };

            AsyncTcpConnector.Connect(ip, port, new Queue<TimeSpan>(timeout),
                (bool isConnected, Socket newSocket, object initialData) =>
                {
                    Assert.IsTrue(isConnected, "���� ����");

                    exceptionThrower = new AsyncTcpConnection(newSocket);
                    exceptionThrower.Subscribe(
                        onReceived: data => throw new Exception("�޼��� �Ľ�"),
                        onError: error => isExceptionTerminated = true,
                        onCompleted: () => Assert.Fail("����"));
                });

            TestHelper.BecomeTrue(() =>
            {
                return exceptionThrower != null 
                    && oppositeConnection != null;
            }, TimeSpan.FromMilliseconds(3000)).Wait();

            oppositeConnection.Send(new ArraySegment<byte>(Encoding.UTF8.GetBytes("����")));

            TestHelper.BecomeTrue(() =>
            {
                return (isExceptionTerminated == true) &&
                       (isOppositeTerminated);
            }, TimeSpan.FromSeconds(2)).Wait();
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