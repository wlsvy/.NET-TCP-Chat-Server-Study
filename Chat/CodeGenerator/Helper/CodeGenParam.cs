using System.Text;

namespace CodeGenerator.Helper
{
    internal struct CodeGenParam
    {
        public readonly string TypeName;
        public readonly string ParameterName;
        public readonly string DefaultValue;

        public CodeGenParam(string typeName, string paramName, string defaultValue = null)
        {
            TypeName = typeName;
            ParameterName = paramName;
            DefaultValue = defaultValue ?? string.Empty;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(DefaultValue))
            {
                return $"{TypeName} {ParameterName}";
            }
            else
            {
                return $"{TypeName} {ParameterName} = {DefaultValue}";
            }
        }

        public static string Concat(CodeGenParam[] parameters)
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
