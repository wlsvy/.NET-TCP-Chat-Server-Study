using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core
{
    public sealed class ServerConfig
    {
        public string CSListenIPAddress { get; set; }
        public int CSListenPort { get; set; }
        public int TimeSlicePerUpdateMSec { get; set; }

    }
}
