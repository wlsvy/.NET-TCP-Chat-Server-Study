using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    internal static class CodoGenUtil
    {
        public static ProtocolContent.ProtocolDirection GetDirection(string direction)
        {
            if(direction == ProtocolContent.ProtocolDirection.CS.ToString())
            {
                return ProtocolContent.ProtocolDirection.CS;
            }
            else if (direction == ProtocolContent.ProtocolDirection.SC.ToString())
            {
                return ProtocolContent.ProtocolDirection.SC;
            }
            else
            {
                throw new ArgumentException(direction);
            }
        }
    }
}
