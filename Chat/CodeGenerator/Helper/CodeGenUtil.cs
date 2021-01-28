using CodeGenerator.Protocol;
using System;

namespace CodeGenerator.Helper
{
    internal static class CodeGenUtil
    {
        public static ProtocolContent.ProtocolDirection GetDirection(string direction)
        {
            if (direction == ProtocolContent.ProtocolDirection.CS.ToString())
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

        public static string GetNamespaceFromDirectory(string directoryPath)
        {
            var pos = directoryPath.IndexOf(Global.ROOT_DIRECTORY_NAME);
            if(pos == -1)
            {
                return null;
            }

            pos += Global.ROOT_DIRECTORY_NAME.Length;
            pos += 1; //구분자 포함.
            if(pos >= directoryPath.Length)
            {
                return null;
            }

            return directoryPath.Substring(pos).Replace("\\", ".");
        }
    }
}
