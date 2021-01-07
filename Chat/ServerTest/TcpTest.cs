using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Sockets;
using Shared.Logger;
using Shared.Network;
using Shared.Util;

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

            AsyncTcpConnection oppositeConnection;

            var acceptor = new AsyncTcpAcceptor(
                onNewConnection: (accepted) =>
                {
                    oppositeConnection = new AsyncTcpConnection(
                        socket: accepted, 
                        onSendError: (exception) => logger.Error("On Send Error", exception));

                },
                logger: logger);

        }
    }
}
