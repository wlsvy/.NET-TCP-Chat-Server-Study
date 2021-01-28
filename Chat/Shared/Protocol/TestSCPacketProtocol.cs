namespace Shared.Protocol
{
    public enum TestSCPacketProtocol : byte
    {
        Invalid,
        SC_Pong,
        SC_Login,
        SC_CreateAccount,
        SC_ChatMessage,
    }
}
