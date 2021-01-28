using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGenerator.Helper;
using CodeGenerator.Writer;

namespace CodeGenerator.Protocol
{
    internal static class ProtocolCodeBuilder
    {
        public static IReadOnlyList<CodeGenContext> GenerateCode(IReadOnlyList<ProtocolContent> protocolContents)
        {
            var result = new List<CodeGenContext>();

            var groups = from p in protocolContents
                         group p by p.Direction;

            foreach(var group in groups)
            {
                result.Add(BuildPacketProtocol(group, group.Key));
            }

            return result;
        }

        private static CodeGenContext BuildPacketProtocol(IEnumerable<ProtocolContent> protocolContents, ProtocolContent.ProtocolDirection direction)
        {
            var protocolPath = Global.DIRECTORY_DIC[Global.Directories.Shared_Protocol];
            var directoryNamespace = CodeGenUtil.GetNamespaceFromDirectory(protocolPath) ?? throw new DirectoryNotFoundException();
            var typename = $"{direction}PacketProtocol";
            var result = new CodeGenContext(directoryPath: protocolPath, fileName: $"{typename}.cs");

            using (var n = BlockWriter.Namespace(result, directoryNamespace))
            {
                using (var e = BlockWriter.Enum(result, AccessModifier.Public, typename, "byte"))
                {
                    result.AppendLine($"Invalid,");

                    foreach (var p in protocolContents)
                    {
                        result.AppendLine($"{direction}_{p.ProtocolName},");
                    }
                }
            }
            return result;
        }
    }
}
