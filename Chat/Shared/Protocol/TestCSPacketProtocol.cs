namespace Shared.Protocol
{
    public enum TestCSPacketProtocol : byte
    {
        Invalid,
        CS_Ping,
        CS_Login,
        CS_CreateAccount,
        CS_ChatMessage,
    }
}
