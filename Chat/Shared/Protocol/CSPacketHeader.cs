namespace Shared.Protocol
{
    public readonly struct CSPacketHeader
    {
        public static readonly int HEADER_SIZE = sizeof(int) + sizeof(int);
        public static readonly CSPacketHeader Invalid = new CSPacketHeader(CSPacketProtocol.Invalid, 0);

        public readonly CSPacketProtocol Protocol;
        public readonly int BodySize;

        public CSPacketHeader(CSPacketProtocol protocol, int bodySize)
        {
            Protocol = protocol;
            BodySize = bodySize;
        }
    }
}
