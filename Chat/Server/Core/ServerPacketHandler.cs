using Shared.Logger;
using Shared.Network;
using Shared.Protocol;

namespace Server.Core
{
    public sealed class ServerPacketHandler : PacketHandler
    {
        public override void HANDLE_SC_Ping(long sequenceNumber)
        {
            Log.I.Info($"Network Ping : {sequenceNumber}");
        }
    }
}
