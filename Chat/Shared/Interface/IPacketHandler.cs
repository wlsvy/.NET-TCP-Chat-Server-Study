namespace Shared.Interface
{
    public interface IPacketHandler
    {
        void HANDLE_SC_Ping(long sequenceNumber);
    }
}
