using System;

namespace CodeGenerator
{
    internal readonly struct ProtocolContent
    {
        public enum ContentsGroupAttribue
        {
            name,
        }
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
        public readonly (Type paramType, string paramName)[] Parameters;

        public ProtocolContent(string name, ProtocolDirection direction, (Type paramType, string paramName)[] parameters)
        {
            ProtocolName = name;
            Direction = direction;
            Parameters = parameters;
        }
    }
}
