namespace Shared.Protocol
{
    public interface ISCPacketHandler
    {
        void HANDLE_SC_Ping_NTF(long sequenceNumber) { }
        void HANDLE_SC_Login_RSP(long accountId) { }
        void HANDLE_SC_CreateAccount_RSP(long accountId) { }
    }
}
