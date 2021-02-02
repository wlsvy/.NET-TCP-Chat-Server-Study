using System;

namespace CodeGenerator.Protocol
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
            defaultValue,
        }
        
        public enum ProtocolDirection
        {
            CS,
            SC,
        }

        public readonly string ProtocolName;
        public readonly string Direction;
        public readonly CodeGenParam[] Parameters;

        public ProtocolContent(string name, string direction, params CodeGenParam[] parameters)
        {
            if(!Enum.TryParse<ProtocolDirection>(direction, out _))
            {
                throw new ArgumentException(nameof(direction));
            }

            ProtocolName = name;
            Direction = direction;
            Parameters = parameters;
        }
    }
}
