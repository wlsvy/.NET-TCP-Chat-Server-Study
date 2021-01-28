using System.Text;

namespace CodeGenerator.Helper
{
    internal struct CodeGenParam
    {
        public readonly string TypeName;
        public readonly string ParameterName;
        public readonly string ParameterModifier;
        public readonly string DefaultValue;

        public CodeGenParam(string typeName, string paramName, string paramModifier = null, string defaultValue = null)
        {
            TypeName = typeName;
            ParameterName = paramName;
            ParameterModifier = paramModifier ?? string.Empty;
            DefaultValue = defaultValue ?? string.Empty;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(DefaultValue))
            {
                return $"{TypeName} {ParameterModifier}{ParameterName}";
            }
            else
            {
                return $"{TypeName} {ParameterModifier}{ParameterName} = {DefaultValue}";
            }
        }
    }

    internal static class ParameterExtension
    {
        public static string Concat(this CodeGenParam[] parameters)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < parameters.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(parameters[i].ToString());
            }
            return builder.ToString();
        }
    }
}
