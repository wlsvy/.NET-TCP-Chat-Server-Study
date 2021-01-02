namespace Shared.Protocol.SC
{
    public readonly struct SCPacketHeader
    {
        public static readonly int HEADER_SIZE = sizeof(int) + sizeof(int);
        public static readonly SCPacketHeader Invalid = new SCPacketHeader(SCProtocol.Invalid, 0);

        public readonly SCProtocol Protocol;
        public readonly int BodySize;

        public SCPacketHeader(SCProtocol protocol, int bodySize)
        {
            Protocol = protocol;
            BodySize = bodySize;
        }
    }
}
