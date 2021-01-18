namespace Shared.Protocol
{
    public enum PacketProtocol : byte
    {
        Invalid,

        SC_Ping,
        CS_Pong,

        CS_Message

    }
}
