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
                result.Add(BuildPacketPacker(group, group.Key));
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
                using (var e = BlockWriter.Enum(result, AccessModifier.Public, newTypename, BaseTypes.INT))
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
                using (var i = BlockWriter.Interface(result, AccessModifier.Public, newTypename))
                {
                    foreach (var p in protocolContents)
                    {
                        LineWriter.InterfaceMethod(result, BaseTypes.VOID, $"HANDLE_{direction}_{p.ProtocolName}", p.Parameters);
                    }
                }
            }
            return result;
        }

        private static CodeGenContext BuildPacketPacker(IEnumerable<ProtocolContent> protocolContents, ProtocolContent.ProtocolDirection direction)
        {
            var protocolPath = Global.DIRECTORY_DIC[Global.Directories.Shared_Protocol];
            var directoryNamespace = CodeGenUtil.GetNamespaceFromDirectory(protocolPath) ?? throw new DirectoryNotFoundException();
            var newTypename = $"{direction}PacketPacker";
            var protocolEnumType = $"{direction}PacketProtocol";
            var result = new CodeGenContext(directoryPath: protocolPath, fileName: $"{newTypename}.cs");

            LineWriter.CodeGenCaption(result);
            LineWriter.LineSpace(result);

            LineWriter.UsingNamespace(result, "Shared.Network");
            LineWriter.UsingNamespace(result, "Shared.Util");
            LineWriter.UsingNamespace(result, "System");
            LineWriter.LineSpace(result);

            using (var n = BlockWriter.Namespace(result, directoryNamespace))
            {
                using (var c = BlockWriter.Class(result, AccessModifier.Internal, ClassModifier.Static, newTypename))
                {
                    using (var m = BlockWriter.Method(result, AccessModifier.Private, MethodModifier.Static, BaseTypes.VOID, "WriteHeader",
                        new ProtocolParameter("BinaryEncoder", "encoder"),
                        new ProtocolParameter(protocolEnumType, "protocol"),
                        new ProtocolParameter(BaseTypes.INT, "bodySize")))
                    {
                        LineWriter.Line(result, "encoder.Write(in protocol);");
                        LineWriter.Line(result, "encoder.Write(in bodySize);");
                    }


                    foreach (var p in protocolContents)
                    {
                        using (var m = BlockWriter.Method(result, AccessModifier.Public, MethodModifier.Static, "ArraySegment<byte>", $"Pack_{direction}_{p.ProtocolName}", p.Parameters))
                        {
                            LineWriter.Line(result, $"var protocol = {protocolEnumType}.{direction}_{p.ProtocolName};");

                            LineWriter.LineSpace(result);

                            LineWriter.Line(result, $"int bodySize = 0;");
                            foreach(var param in p.Parameters)
                            {
                                LineWriter.Line(result, $"bodySize += {param.ParameterName}.SizeForWrite();");
                            }
                            LineWriter.LineSpace(result);

                            LineWriter.Line(result, $"int packetSize = {direction}PacketHeader.HEADER_SIZE + bodySize;");
                            LineWriter.LineSpace(result);

                            LineWriter.Line(result, $"var packetBuffer = new ArraySegment<byte>(new byte[packetSize]);");
                            using (var u = BlockWriter.Using(result, "encoder", "BinaryEncoder", "packetBuffer"))
                            {
                                LineWriter.Line(result, $"WriteHeader(encoder, protocol, bodySize);");
                                foreach (var param in p.Parameters)
                                {
                                    LineWriter.Line(result, $"encoder.Write(in {param.ParameterName});");
                                }
                            }
                            LineWriter.Line(result, $"return packetBuffer;");
                        }
                    }
                }
            }
            return result;
        }

        private static object CSPacketProtocol()
        {
            throw new System.NotImplementedException();
        }
    }
}
