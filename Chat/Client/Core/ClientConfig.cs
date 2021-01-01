using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core
{
    public sealed class ClientConfig
    {
        public string ServerIPAddress { get; set; }
        public int ServerPort { get; set; }
        public int TimeSlicePerUpdateMSec { get; set; }

    }
}
