using CodeGenerator.Helper;
using CodeGenerator.Writer;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                result.Add(BuildPacketHandlerInterface(group, group.Key));
            }

            return result;
        }

        private static CodeGenContext BuildPacketProtocol(IEnumerable<ProtocolContent> protocolContents, ProtocolContent.ProtocolDirection direction)
        {
            var protocolPath = Global.DIRECTORY_DIC[Global.Directories.Shared_Protocol];
            var directoryNamespace = CodeGenUtil.GetNamespaceFromDirectory(protocolPath) ?? throw new DirectoryNotFoundException();
            var newTypename = $"{direction}PacketProtocol";
            var result = new CodeGenContext(directoryPath: protocolPath, fileName: $"{newTypename}.cs");

            LineWriter.CodeGenCaption(result);
            using (var n = BlockWriter.Namespace(result, directoryNamespace))
            {
                using (var e = BlockWriter.Enum(result, AccessModifier.Public, newTypename, "byte"))
                {
                    LineWriter.Line(result, $"Invalid,");
                    LineWriter.LineSpace(result);
                    foreach (var p in protocolContents)
                    {
                        LineWriter.Line(result, $"{direction}_{p.ProtocolName},");
                    }
                }
            }
            return result;
        }

        private static CodeGenContext BuildPacketHandlerInterface(IEnumerable<ProtocolContent> protocolContents, ProtocolContent.ProtocolDirection direction)
        {
            var protocolPath = Global.DIRECTORY_DIC[Global.Directories.Shared_Protocol];
            var directoryNamespace = CodeGenUtil.GetNamespaceFromDirectory(protocolPath) ?? throw new DirectoryNotFoundException();
            var newTypename = $"I{direction}PacketHandler";
            var result = new CodeGenContext(directoryPath: protocolPath, fileName: $"{newTypename}.cs");

            LineWriter.CodeGenCaption(result);
            using (var n = BlockWriter.Namespace(result, directoryNamespace))
            {
                using (var e = BlockWriter.Interface(result, AccessModifier.Public, newTypename))
                {
                    foreach (var p in protocolContents)
                    {
                        LineWriter.InterfaceMethod(result, "void", $"HANDLE_{direction}_{p.ProtocolName}", p.Parameters);
                    }
                }
            }
            return result;
        }
    }
}
