namespace Shared.Protocol.CS
{
    public readonly struct CSPacketHeader
    {
        public static readonly int HEADER_SIZE = sizeof(int) + sizeof(int);
        public static readonly CSPacketHeader Invalid = new CSPacketHeader(CSProtocol.Invalid, 0);

        public readonly CSProtocol Protocol;
        public readonly int BodySize;

        public CSPacketHeader(CSProtocol protocol, int bodySize)
        {
            Protocol = protocol;
            BodySize = bodySize;
        }
    }
}
