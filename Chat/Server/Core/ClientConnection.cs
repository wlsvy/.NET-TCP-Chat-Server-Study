using System.Net;
using System.Net.Sockets;

namespace Server.Core
{
    public sealed class ClientConnection
    {
        public readonly long Id;


        public ClientConnection(long id, Socket tcpSocket, SessionManager sessionManager)
        {

        }
    }
}
