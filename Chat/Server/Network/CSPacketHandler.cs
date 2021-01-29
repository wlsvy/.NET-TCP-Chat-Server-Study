using Shared.Logger;
using Shared.Protocol;

namespace Server.Network
{
    public sealed class CSPacketHandler : ICSPacketHandler
    {
        public void HANDLE_CS_Ping(long sequenceNumber)
        {
            Log.I.Info($"Network Ping : {sequenceNumber}");
        }

        public void HANDLE_CS_Login(string id, string password)
        {
            Log.I.Info($"id {id}, password {password}");
        }

        public void HANDLE_CS_CreateAccount(string id, string password)
        {
            Log.I.Info($"id {id}, password {password}");
        }

        public void HANDLE_CS_ChatMessage(string message)
        {

        }
    }
}
