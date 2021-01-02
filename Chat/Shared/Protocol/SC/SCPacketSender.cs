using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Protocol.SC
{
    public sealed class SCPacketSender
    {
        public void SEND_SYSTEM_TEST_PINT(long sequenceNumber)
        {
            //var packet = GCPacketPacker.PACK_SYSTEM_Ping_NTF(sequenceNumber);
            //this.connection.Send(packet);
        }
    }
}
