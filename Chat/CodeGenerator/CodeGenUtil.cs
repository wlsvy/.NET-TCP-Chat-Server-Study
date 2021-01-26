using System;

namespace CodeGenerator
{
    public enum ContentsGroupAttribue
    {
        name,
    }
    public enum ContentsGroupType
    {
        Protocol
    }

    internal static class CodeGenUtil
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
