namespace Shared.Protocol
{
    public enum SCPacketProtocol : byte
    {
        Invalid,
        SC_Ping_NTF,
        SC_Login_RSP,
        SC_CreateAccount_RSP,
        SC_ChatMessage_RSP
    }
}
