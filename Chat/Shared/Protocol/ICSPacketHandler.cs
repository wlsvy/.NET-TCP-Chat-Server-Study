namespace Shared.Protocol
{
    public interface ICSPacketHandler
    {
        void HANDLE_CS_Login_REQ(string id, string password) { }
        void HANDLE_CS_Pong_NTF(long sequenceNumber) { }
        void HANDLE_CS_CreateAccount_REQ(string id, string password) { }
    }
}
