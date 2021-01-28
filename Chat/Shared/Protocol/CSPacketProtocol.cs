namespace Shared.Protocol
{
    public enum CSPacketProtocol : byte
    {
        Invalid,
        CS_Pong_NTF,
        CS_Login_REQ,
        CS_CreateAccount_REQ,
        CS_ChatMessage_REQ,
    }
}
