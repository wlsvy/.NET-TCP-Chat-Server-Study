namespace Shared.Protocol
{
    public abstract class PacketHandler
    {
        public virtual void HANDLE_SC_Ping(long sequenceNumber) { }
    }
}
