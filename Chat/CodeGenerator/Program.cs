using CodeGenerator.Helper;
using CodeGenerator.Protocol;
using System;

namespace CodeGenerator
{
    internal static class Program
    {
        public const string ROOT_DIRECTORY_NAME = ".NET-TCP-Chat-Server-Study";

        static void Main(string[] args)
        {
            CodeGenUtil.Initialize();

            var docs = SchemaLoader.LoadXmlSchema("Schema");

            var protocolsContents = ProtocolContentParser.Parse(docs);

            var codeGenContext = ProtocolCodeBuilder.GenerateCode(protocolsContents);

            CodeGenExporter.Export(codeGenContext);

            Console.WriteLine("코드젠 완료");
        }
    }
}
