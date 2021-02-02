using CodeGenerator.Helper;
using CodeGenerator.Protocol;
using System;

namespace CodeGenerator
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            CodeGenUtil.Initialize();

            var docs = SchemaLoader.LoadXmlSchema("Schema");
            var protocolsContents = ProtocolContentParser.Parse(docs);
            var codeGenContexts = ProtocolCodeBuilder.GenerateCode(protocolsContents);
            CodeGenExporter.Export(codeGenContexts);

            Console.WriteLine("코드젠 완료");
        }
    }
}
