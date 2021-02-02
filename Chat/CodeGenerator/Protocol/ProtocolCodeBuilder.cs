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

            foreach (var group in groups)
            {
                result.Add(BuildPacketProtocol(group, group.Key));
                result.Add(BuildPacketHandlerInterface(group, group.Key));
                result.Add(BuildPacketPacker(group, group.Key));
                result.Add(BuildPacketProcessor(group, group.Key));
                result.Add(BuildPacketSender(group, group.Key));
            }

            return result;
        }

        private static CodeGenContext BuildPacketProtocol(IEnumerable<ProtocolContent> protocolContents, ProtocolContent.ProtocolDirection direction)
        {
            var protocolPath = Global.DIRECTORY_DIC[Global.Directories.Shared_Protocol];
            var directoryNamespace = CodeGenUtil.GetNamespaceFromDirectory(protocolPath) ?? throw new DirectoryNotFoundException();
            var newTypename = $"{direction}PacketProtocol";

            var code = new CodeGenContext(directoryPath: protocolPath, fileName: $"{newTypename}.cs");

            using (code.Scope($"namespace {directoryNamespace}"))
            {
                using (code.Scope($"public enum {newTypename}: int"))
                {
                    code.Line($"Invalid,");
                    code.LineSpcae();

                    foreach (var p in protocolContents)
                    {
                        code.Line($"{direction}_{p.ProtocolName},");
                    }
                }
            }
            return code;
        }

        private static CodeGenContext BuildPacketHandlerInterface(IEnumerable<ProtocolContent> protocolContents, ProtocolContent.ProtocolDirection direction)
        {
            var protocolPath = Global.DIRECTORY_DIC[Global.Directories.Shared_Protocol];
            var directoryNamespace = CodeGenUtil.GetNamespaceFromDirectory(protocolPath) ?? throw new DirectoryNotFoundException();
            var newTypename = $"I{direction}PacketHandler";

            var code = new CodeGenContext(directoryPath: protocolPath, fileName: $"{newTypename}.cs");

            using (code.Scope($"namespace {directoryNamespace}"))
            {
                using (code.Scope($"public interface {newTypename}"))
                {
                    foreach (var p in protocolContents)
                    {
                        code.Line($"void HANDLE_{direction}_{p.ProtocolName}({ProtocolParameter.Concat(p.Parameters)});");
                    }
                }
            }
            return code;
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
                using (var c = BlockWriter.Class(result, AccessModifier.Public, ClassModifier.Static, newTypename))
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
                            foreach (var param in p.Parameters)
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

        private static CodeGenContext BuildPacketProcessor(IEnumerable<ProtocolContent> protocolContents, ProtocolContent.ProtocolDirection direction)
        {
            var protocolPath = Global.DIRECTORY_DIC[Global.Directories.Shared_Protocol];
            var directoryNamespace = CodeGenUtil.GetNamespaceFromDirectory(protocolPath) ?? throw new DirectoryNotFoundException();

            var newTypename = $"{direction}PacketProcessor";
            var packetHandlerInterface = $"I{direction}PacketHandler";
            var packetHeader = $"{direction}PacketHeader";
            var packetProtocol = $"{direction}PacketProtocol";

            var result = new CodeGenContext(directoryPath: protocolPath, fileName: $"{newTypename}.cs");

            LineWriter.CodeGenCaption(result);
            LineWriter.LineSpace(result);

            LineWriter.UsingNamespace(result, "Shared.Network");
            LineWriter.UsingNamespace(result, "System");
            LineWriter.UsingNamespace(result, "System.Diagnostics");
            LineWriter.LineSpace(result);

            using (var namespaceBlock = BlockWriter.Namespace(result, directoryNamespace))
            {
                using (var classBlock = BlockWriter.Block(result, $"public sealed class {newTypename} : PacketProcessorBase"))
                {
                    LineWriter.Line(result, $"private readonly {packetHandlerInterface} m_PacketHandler;");
                    LineWriter.LineSpace(result);

                    using (var constructor = BlockWriter.Block(result, $"public {newTypename}({packetHandlerInterface} handler)"))
                    {
                        LineWriter.Line(result, "m_PacketHandler = handler ?? throw new ArgumentNullException(nameof(handler));");
                    }

                    using (var methodBlock = BlockWriter.Block(result, $"public int ParseAndHandlePacket(ArraySegment<byte> dataStream)"))
                    {
                        using (var ifBlock = BlockWriter.Block(result, $"if(!HasPacketHeader(dataStream))"))
                        {
                            LineWriter.Return(result, "0");
                        }
                        LineWriter.LineSpace(result);

                        LineWriter.Line(result, "var packetHeader = ParseHeader(dataStream);");
                        using (var ifBlock = BlockWriter.Block(result, $"if(!HasPacketBody(dataStream, packetHeader))"))
                        {
                            LineWriter.Return(result, "0");
                        }
                        LineWriter.LineSpace(result);

                        LineWriter.Line(result, "var packetBody = PeekPacketBody(dataStream, packetHeader);");
                        LineWriter.Line(result, "ParseAndHandleBody(packetHeader, packetBody);");
                        LineWriter.LineSpace(result);

                        LineWriter.Return(result, $"{packetHeader}.HEADER_SIZE + packetHeader.BodySize");
                    }

                    using (var methodBlock = BlockWriter.Block(result, $"private void ParseAndHandleBody({packetHeader} header, ArraySegment<byte> body)"))
                    {
                        using (var switchBlock = BlockWriter.Block(result, "switch (header.Protocol)"))
                        {
                            foreach (var p in protocolContents)
                            {
                                LineWriter.Line(result, $"case {packetProtocol}.{direction}_{p.ProtocolName}: ParseAndHandle_{direction}_{p.ProtocolName}(body); break;");
                            }
                        }
                    }

                    LineWriter.Line(result, "#region Packet Paser Method");
                    LineWriter.LineSpace(result);

                    using (var methodBlock = BlockWriter.Block(result, $"private static {packetHeader} ParseHeader(ArraySegment<byte> dataStream)"))
                    {
                        LineWriter.Line(result, "Debug.Assert(dataStream.Array != null);");
                        LineWriter.Line(result, "Debug.Assert(HasPacketHeader(dataStream));");
                        LineWriter.LineSpace(result);

                        LineWriter.Line(result, "int number = BitConverter.ToInt32(dataStream.Array, dataStream.Offset);");
                        LineWriter.Line(result, $"var protocol = {packetProtocol}.Invalid;");

                        using (var tryBlock = BlockWriter.Block(result, $"try"))
                        {
                            LineWriter.Line(result, $"protocol = ({packetProtocol})number;");
                        }
                        using (var catchBlock = BlockWriter.Block(result, $"catch (InvalidCastException)"))
                        {
                            LineWriter.Line(result, $"throw;");
                        }
                        LineWriter.LineSpace(result);

                        LineWriter.Line(result, $"int bodySize = BitConverter.ToInt32(dataStream.Array, dataStream.Offset + sizeof(int));");
                        using (var ifBlock = BlockWriter.Block(result, $"if (bodySize < 0)"))
                        {
                            LineWriter.Line(result, $"throw new ArgumentOutOfRangeException($\"패킷 BodySize가 음수입니다. ProtocolNumber[{{number}}] BodySize[{{bodySize}}]\");");
                        }
                        LineWriter.LineSpace(result);

                        LineWriter.Line(result, $"return new {packetHeader}(protocol, bodySize);");
                    }
                    using (var methodBlock = BlockWriter.Block(result, "private static bool HasPacketHeader(ArraySegment<byte> dataStream)"))
                    {
                        LineWriter.Line(result, $"return (dataStream.Count >= {packetHeader}.HEADER_SIZE);");
                    }
                    using (var methodBlock = BlockWriter.Block(result, $"private static bool HasPacketBody(ArraySegment<byte> dataStream, {packetHeader} packetHeader)"))
                    {
                        LineWriter.Line(result, $"return (dataStream.Count >= ({packetHeader}.HEADER_SIZE + packetHeader.BodySize));");
                    }
                    using (var methodBlock = BlockWriter.Block(result, $"private static ArraySegment<byte> PeekPacketBody(ArraySegment<byte> dataStream, {packetHeader} packetHeader)"))
                    {
                        LineWriter.Line(result, $"return new ArraySegment<byte>(dataStream.Array, dataStream.Offset + {packetHeader}.HEADER_SIZE, packetHeader.BodySize);");
                    }
                    LineWriter.LineSpace(result);

                    LineWriter.Line(result, "#endregion");
                    LineWriter.LineSpace(result);

                    LineWriter.Line(result, "#region Packet handler Method");
                    LineWriter.LineSpace(result);

                    foreach (var p in protocolContents)
                    {
                        using (var methodBlock = BlockWriter.Block(result, $"private void ParseAndHandle_{direction}_{p.ProtocolName}(ArraySegment<byte> body)"))
                        {
                            using (var usingBlock = BlockWriter.Block(result, $"using (var reader = new BinaryDecoder(body))"))
                            {
                                foreach (var param in p.Parameters)
                                {
                                    LineWriter.Line(result, $"reader.Read(out {param.TypeName} {param.ParameterName});");
                                }
                                LineWriter.LineSpace(result);

                                using (var lambdaBlock = BlockWriter.Block(result, $"RunOrReserveHandler(handler: async () =>"))
                                {
                                    LineWriter.Line(result, $"m_PacketHandler.HANDLE_{direction}_{p.ProtocolName}({ProtocolParameter.ConcatNames(p.Parameters)});");
                                }
                                LineWriter.AppendPreviousLine(result, ");");
                            }
                        }
                    }
                    LineWriter.LineSpace(result);

                    LineWriter.Line(result, "#endregion");
                }
            }
            return result;
        }

        private static CodeGenContext BuildPacketSender(IEnumerable<ProtocolContent> protocolContents, ProtocolContent.ProtocolDirection direction)
        {
            var protocolPath = Global.DIRECTORY_DIC[Global.Directories.Shared_Protocol];
            var directoryNamespace = CodeGenUtil.GetNamespaceFromDirectory(protocolPath) ?? throw new DirectoryNotFoundException();

            var newTypename = $"{direction}PacketSender";
            var packetPacker = $"{direction}PacketPacker";

            var result = new CodeGenContext(directoryPath: protocolPath, fileName: $"{newTypename}.cs");

            LineWriter.CodeGenCaption(result);
            LineWriter.LineSpace(result);

            LineWriter.Line(result, "using Shared.Network;");
            LineWriter.Line(result, "using System;");
            LineWriter.LineSpace(result);

            using (var namespaceBlock = BlockWriter.Block(result, $"namespace {directoryNamespace}"))
            {
                using (var classBlock = BlockWriter.Block(result, $"public sealed class {newTypename}"))
                {
                    LineWriter.Line(result, "private AsyncTcpConnection m_Connection;");

                    using (var ctorBlock = BlockWriter.Block(result, $"public {newTypename}(AsyncTcpConnection connection)"))
                    {
                        LineWriter.Line(result, "m_Connection = connection ?? throw new ArgumentNullException(nameof(connection));");
                    }
                    LineWriter.LineSpace(result);

                    foreach (var p in protocolContents)
                    {
                        using (var methodBlock = BlockWriter.Block(result, $"public void SEND_{direction}_{p.ProtocolName}({ProtocolParameter.Concat(p.Parameters)})"))
                        {
                            LineWriter.Line(result, $"var packet = {packetPacker}.Pack_{direction}_{p.ProtocolName}({ProtocolParameter.ConcatNames(p.Parameters)});");
                            LineWriter.Line(result, $"m_Connection.Send(packet);");
                        }
                        LineWriter.LineSpace(result);
                    }

                }
            }
            return result;
        }

    }
}
