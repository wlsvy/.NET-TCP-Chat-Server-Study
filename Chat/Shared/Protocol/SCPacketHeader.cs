namespace Shared.Protocol
{
    public readonly struct SCPacketHeader
    {
        public static readonly int HEADER_SIZE = sizeof(int) + sizeof(int);
        public static readonly SCPacketHeader Invalid = new SCPacketHeader(SCPacketProtocol.Invalid, 0);

        public readonly SCPacketProtocol Protocol;
        public readonly int BodySize;

        public SCPacketHeader(SCPacketProtocol protocol, int bodySize)
        {
            Protocol = protocol;
            BodySize = bodySize;
        }
    }
}