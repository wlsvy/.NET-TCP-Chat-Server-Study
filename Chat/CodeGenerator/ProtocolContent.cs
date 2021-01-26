using System;

namespace CodeGenerator
{
    internal readonly struct ProtocolContent
    {
        
        public enum ProtocolAttribute
        {
            name,
            direction,
        }
        public enum ParameterAttribute
        {
            type,
            name,
        }
        
        public enum ProtocolDirection
        {
            CS,
            SC,
        }

        public readonly string ProtocolName;
        public readonly ProtocolDirection Direction;
        public readonly (string paramType, string paramName)[] Parameters;

        public ProtocolContent(string name, ProtocolDirection direction, (string paramType, string paramName)[] parameters)
        {
            ProtocolName = name;
            Direction = direction;
            Parameters = parameters;
        }
    }
}
