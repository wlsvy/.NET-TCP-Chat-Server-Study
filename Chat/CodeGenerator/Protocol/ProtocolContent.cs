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
        public readonly ProtocolDirection Direction;
        public readonly ProtocolParameter[] Parameters;

        public ProtocolContent(string name, ProtocolDirection direction, params ProtocolParameter[] parameters)
        {
            ProtocolName = name;
            Direction = direction;
            Parameters = parameters;
        }
    }
}
