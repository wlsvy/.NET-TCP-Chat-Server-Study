namespace Shared.Protocol
{
    public enum PacketProtocol : byte
    {
        Invalid,

        SC_Ping,
        CS_Pong,

        CS_Login,
        CS_CreateAccount,

        CS_Message

    }
}
