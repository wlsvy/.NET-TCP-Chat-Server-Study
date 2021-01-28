using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Protocol
{
    internal static class ProtocolCodeBuilder
    {
        public static IReadOnlyList<CodeGenContext> GenerateCode(IReadOnlyList<ProtocolContent> protocolContents)
        {
            var result = new List<CodeGenContext>();
            result.AddRange(BuildPacketProtocol(protocolContents));

            return result;
        }

        private static IReadOnlyList<CodeGenContext> BuildPacketProtocol(IReadOnlyList<ProtocolContent> protocolContents)
        {
            var protocolPath = Global.DIRECTORY_DIC[Global.Directories.Shared_Protocol];

            var csPacketProtocol = new CodeGenContext(directoryPath: protocolPath, fileName: "TestCSPacketProtocol.cs");
            var scPacketProtocol = new CodeGenContext(directoryPath: protocolPath, fileName: "TestSCPacketProtocol.cs");

            foreach(var content in protocolContents)
            {
                switch (content.Direction)
                {
                    case ProtocolContent.ProtocolDirection.CS: break;
                    case ProtocolContent.ProtocolDirection.SC: break;
                }
            }

            return new List<CodeGenContext>() { csPacketProtocol, scPacketProtocol };

            void BuildCS(ProtocolContent content)
            {

            }

            void DoBuild()
            {

            }
        }
    }
}
