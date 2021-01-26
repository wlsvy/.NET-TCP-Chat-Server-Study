using System;

namespace CodeGenerator
{
    internal readonly struct ProtocolContent
    {
        public readonly string ProtocolName;
        public readonly (Type paramType, string paramName)[] Parameters;

        public ProtocolContent(string name, (Type paramType, string paramName)[] parameters)
        {
            ProtocolName = name;
            Parameters = parameters;
        }
    }
}
