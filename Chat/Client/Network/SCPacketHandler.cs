using Shared.Logger;
using Shared.Protocol;

namespace Client.Network
{
    public sealed class SCPacketHandler : ISCPacketHandler
    {
        public void HANDLE_SC_Pong(long sequenceNumber)
        {
            Log.I.Info($"Network Pong : {sequenceNumber}");
        }

        public void HANDLE_SC_Login(long accountId)
        {
            Log.I.Info($"Network HANDLE_SC_Login_RSP : {accountId}");
            
        }

        public void HANDLE_SC_CreateAccount(long accountId)
        {
            Log.I.Info($"Network HANDLE_SC_CreateAccount_RSP : {accountId}");
        }

        public void HANDLE_SC_ChatMessage()
        {

        }
    }
}
