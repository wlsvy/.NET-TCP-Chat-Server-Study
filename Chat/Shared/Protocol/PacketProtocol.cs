namespace Shared.Protocol
{
    public enum PacketProtocol : byte
    {
        Invalid,

        SC_Ping_NTF,
        CS_Pong_NTF,

        CS_Login_REQ,
        SC_Login_RSP,
        
        CS_CreateAccount_REQ,
        SC_CreateAccount_RSP,

        CS_ChatMessage_REQ,
        SC_ChatMessage_RSP
    }
}
