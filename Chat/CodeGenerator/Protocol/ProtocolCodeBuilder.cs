using CodeGenerator.Helper;
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
                result.Add(BuildPacketProtocolEnum(group, group.Key));
                result.Add(BuildPacketHandlerInterface(group, group.Key));
                result.Add(BuildPacketPacker(group, group.Key));
                result.Add(BuildPacketProcessor(group, group.Key));
                result.Add(BuildPacketSender(group, group.Key));
            }

            return result;
        }

        private static CodeGenContext BuildPacketProtocolEnum(IEnumerable<ProtocolContent> protocolContents, string direction)
        {
            var protocolPath = CodeGenUtil.DIRECTORY_DIC[CodeGenUtil.Directories.Shared_Protocol];
            var directoryNamespace = CodeGenUtil.GetNamespaceFromDirectory(protocolPath) ?? throw new DirectoryNotFoundException();

            var packetProtocolEnum = $"{direction}PacketProtocol";

            var code = new CodeGenContext(directoryPath: protocolPath, fileName: $"{packetProtocolEnum}.g.cs");

            using (code.Scope($"namespace {directoryNamespace}"))
            {
                using (code.Scope($"public enum {packetProtocolEnum}: int"))
                {
                    code.Line($"Invalid,");
                    code.LineSpace();

                    foreach (var p in protocolContents)
                    {
                        code.Line($"{direction}_{p.ProtocolName},");
                    }
                }
            }
            return code;
        }

        private static CodeGenContext BuildPacketHandlerInterface(IEnumerable<ProtocolContent> protocolContents, string direction)
        {
            var protocolPath = CodeGenUtil.DIRECTORY_DIC[CodeGenUtil.Directories.Shared_Protocol];
            var directoryNamespace = CodeGenUtil.GetNamespaceFromDirectory(protocolPath) ?? throw new DirectoryNotFoundException();

            var packetHandlerInterface = $"I{direction}PacketHandler";

            var code = new CodeGenContext(directoryPath: protocolPath, fileName: $"{packetHandlerInterface}.g.cs");

            using (code.Scope($"namespace {directoryNamespace}"))
            {
                using (code.Scope($"public interface {packetHandlerInterface}"))
                {
                    foreach (var p in protocolContents)
                    {
                        code.Line($"void HANDLE_{direction}_{p.ProtocolName}({p.Parameters.Concat()});");
                    }
                }
            }
            return code;
        }

        private static CodeGenContext BuildPacketPacker(IEnumerable<ProtocolContent> protocolContents, string direction)
        {
            var protocolPath = CodeGenUtil.DIRECTORY_DIC[CodeGenUtil.Directories.Shared_Protocol];
            var directoryNamespace = CodeGenUtil.GetNamespaceFromDirectory(protocolPath) ?? throw new DirectoryNotFoundException();

            var packetPacker = $"{direction}PacketPacker";
            var protocolEnumType = $"{direction}PacketProtocol";

            var code = new CodeGenContext(directoryPath: protocolPath, fileName: $"{packetPacker}.g.cs");

            code.Line($"using Shared.Network;");
            code.Line($"using Shared.Util;");
            code.Line($"using System;");
            code.LineSpace();

            using (code.Scope($"namespace {directoryNamespace}"))
            {
                using (code.Scope($"public static class {packetPacker}"))
                {
                    using(code.Scope($"private static void WriteHeader(BinaryEncoder encoder, {protocolEnumType} protocol, int bodySize)"))
                    {
                        code.Line("encoder.Write(in protocol);");
                        code.Line("encoder.Write(in bodySize);");
                    }

                    foreach (var p in protocolContents)
                    {
                        using (code.Scope($"public static ArraySegment<byte> Pack_{direction}_{p.ProtocolName}({p.Parameters.Concat()})"))
                        {
                            code.Line($"var protocol = {protocolEnumType}.{direction}_{p.ProtocolName};");
                            code.LineSpace();

                            code.Line($"int bodySize = 0;");
                            foreach (var param in p.Parameters)
                            {
                                code.Line($"bodySize += {param.ParameterName}.SizeForWrite();");
                            }
                            code.LineSpace();

                            code.Line($"int packetSize = {direction}PacketHeader.HEADER_SIZE + bodySize;");
                            code.LineSpace();

                            code.Line($"var packetBuffer = new ArraySegment<byte>(new byte[packetSize]);");
                            using (code.Scope($"using(var encoder = new BinaryEncoder(packetBuffer))"))
                            {
                                code.Line($"WriteHeader(encoder, protocol, bodySize);");
                                foreach (var param in p.Parameters)
                                {
                                    code.Line($"encoder.Write(in {param.ParameterName});");
                                }
                            }
                            code.Line($"return packetBuffer;");
                        }
                    }
                }
            }
            return code;
        }

        private static CodeGenContext BuildPacketProcessor(IEnumerable<ProtocolContent> protocolContents, string direction)
        {
            var protocolPath = CodeGenUtil.DIRECTORY_DIC[CodeGenUtil.Directories.Shared_Protocol];
            var directoryNamespace = CodeGenUtil.GetNamespaceFromDirectory(protocolPath) ?? throw new DirectoryNotFoundException();

            var packetProcessor = $"{direction}PacketProcessor";
            var packetHandlerInterface = $"I{direction}PacketHandler";
            var packetHeader = $"{direction}PacketHeader";
            var packetProtocol = $"{direction}PacketProtocol";

            var code = new CodeGenContext(directoryPath: protocolPath, fileName: $"{packetProcessor}.g.cs");

            code.Line($"using Shared.Network;");
            code.Line($"using System;");
            code.Line($"using System.Diagnostics;");
            code.LineSpace();

            using (code.Scope($"namespace {directoryNamespace}"))
            {
            using (code.Scope($"public sealed class {packetProcessor} : PacketProcessorBase"))
                {
                    code.Line($"private readonly {packetHandlerInterface} m_PacketHandler;");
                    code.LineSpace();

                    using (code.Scope($"public {packetProcessor}({packetHandlerInterface} handler)"))
                    {
                        code.Line("m_PacketHandler = handler ?? throw new ArgumentNullException(nameof(handler));");
                    }

                    using (code.Scope($"public int ParseAndHandlePacket(ArraySegment<byte> dataStream)"))
                    {
                        using (code.Scope($"if(!HasPacketHeader(dataStream))"))
                        {
                            code.Line($"return 0;");
                        }
                        code.LineSpace();

                        code.Line("var packetHeader = ParseHeader(dataStream);");
                        using (code.Scope($"if(!HasPacketBody(dataStream, packetHeader))"))
                        {
                            code.Line($"return 0;");
                        }
                        code.LineSpace();

                        code.Line("var packetBody = PeekPacketBody(dataStream, packetHeader);");
                        code.Line("ParseAndHandleBody(packetHeader, packetBody);");
                        code.LineSpace();

                        code.Line($"return {packetHeader}.HEADER_SIZE + packetHeader.BodySize;");
                    }

                    using (code.Scope($"private void ParseAndHandleBody({packetHeader} header, ArraySegment<byte> body)"))
                    {
                        using (code.Scope("switch (header.Protocol)"))
                        {
                            foreach (var p in protocolContents)
                            {
                                code.Line($"case {packetProtocol}.{direction}_{p.ProtocolName}: ParseAndHandle_{direction}_{p.ProtocolName}(body); break;");
                            }
                        }
                    }

                    code.Line("#region Packet Paser Method");
                    code.LineSpace();

                    using (code.Scope($"private static {packetHeader} ParseHeader(ArraySegment<byte> dataStream)"))
                    {
                        code.Line("Debug.Assert(dataStream.Array != null);");
                        code.Line("Debug.Assert(HasPacketHeader(dataStream));");
                        code.LineSpace();

                        code.Line("int number = BitConverter.ToInt32(dataStream.Array, dataStream.Offset);");
                        code.Line($"var protocol = {packetProtocol}.Invalid;");

                        using (code.Scope($"try"))
                        {
                            code.Line($"protocol = ({packetProtocol})number;");
                        }
                        using (code.Scope($"catch (InvalidCastException)"))
                        {
                            code.Line($"throw;");
                        }
                        code.LineSpace();

                        code.Line($"int bodySize = BitConverter.ToInt32(dataStream.Array, dataStream.Offset + sizeof(int));");
                        using (code.Scope($"if (bodySize < 0)"))
                        {
                            code.Line($"throw new ArgumentOutOfRangeException($\"패킷 BodySize가 음수입니다. ProtocolNumber[{{number}}] BodySize[{{bodySize}}]\");");
                        }
                        code.LineSpace();

                        code.Line($"return new {packetHeader}(protocol, bodySize);");
                    }
                    using (code.Scope("private static bool HasPacketHeader(ArraySegment<byte> dataStream)"))
                    {
                        code.Line($"return (dataStream.Count >= {packetHeader}.HEADER_SIZE);");
                    }
                    using (code.Scope($"private static bool HasPacketBody(ArraySegment<byte> dataStream, {packetHeader} packetHeader)"))
                    {
                        code.Line($"return (dataStream.Count >= ({packetHeader}.HEADER_SIZE + packetHeader.BodySize));");
                    }
                    using (code.Scope($"private static ArraySegment<byte> PeekPacketBody(ArraySegment<byte> dataStream, {packetHeader} packetHeader)"))
                    {
                        code.Line($"return new ArraySegment<byte>(dataStream.Array, dataStream.Offset + {packetHeader}.HEADER_SIZE, packetHeader.BodySize);");
                    }
                    code.LineSpace();

                    code.Line("#endregion");
                    code.LineSpace();

                    code.Line("#region Packet handler Method");
                    code.LineSpace();

                    foreach (var p in protocolContents)
                    {
                        using (code.Scope($"private void ParseAndHandle_{direction}_{p.ProtocolName}(ArraySegment<byte> body)"))
                        {
                            using (code.Scope($"using (var reader = new BinaryDecoder(body))"))
                            {
                                foreach (var param in p.Parameters)
                                {
                                    code.Line($"reader.Read(out {param.TypeName} {param.ParameterName});");
                                }
                                code.LineSpace();

                                using (code.Scope($"RunOrReserveHandler(handler: async () =>"))
                                {
                                    code.Line($"m_PacketHandler.HANDLE_{direction}_{p.ProtocolName}({p.Parameters.ConcatName()});");
                                }
                                code.Line(");");
                            }
                        }
                    }
                    code.LineSpace();

                    code.Line("#endregion");
                }
            }
            return code;
        }

        private static CodeGenContext BuildPacketSender(IEnumerable<ProtocolContent> protocolContents, string direction)
        {
            var protocolPath = CodeGenUtil.DIRECTORY_DIC[CodeGenUtil.Directories.Shared_Protocol];
            var directoryNamespace = CodeGenUtil.GetNamespaceFromDirectory(protocolPath) ?? throw new DirectoryNotFoundException();

            var packetSender = $"{direction}PacketSender";
            var packetPacker = $"{direction}PacketPacker";

            var code = new CodeGenContext(directoryPath: protocolPath, fileName: $"{packetSender}.g.cs");

            code.Line("using Shared.Network;");
            code.Line("using System;");
            code.LineSpace();

            using (code.Scope($"namespace {directoryNamespace}"))
            {
                using (code.Scope($"public sealed class {packetSender}"))
                {
                    code.Line("private AsyncTcpConnection m_Connection;");

                    using (code.Scope($"public {packetSender}(AsyncTcpConnection connection)"))
                    {
                        code.Line("m_Connection = connection ?? throw new ArgumentNullException(nameof(connection));");
                    }
                    code.LineSpace();

                    foreach (var p in protocolContents)
                    {
                        using (code.Scope($"public void SEND_{direction}_{p.ProtocolName}({p.Parameters.Concat()})"))
                        {
                            code.Line($"var packet = {packetPacker}.Pack_{direction}_{p.ProtocolName}({p.Parameters.ConcatName()});");
                            code.Line($"m_Connection.Send(packet);");
                        }
                        code.LineSpace();
                    }
                }
            }
            return code;
        }
    }
}
