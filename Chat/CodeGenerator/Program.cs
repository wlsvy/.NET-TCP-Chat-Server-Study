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

            try
            {
                var docs = SchemaLoader.LoadXmlSchema("Schema");
                var protocolsContents = ProtocolContentParser.Parse(docs);
                var codeGenContexts = ProtocolCodeBuilder.GenerateCode(protocolsContents);
                CodeGenExporter.Export(codeGenContexts);
            }
            catch(Exception e)
            {
                Environment.Exit(-1);
            }
            Environment.Exit(0);
        }
    }
}
