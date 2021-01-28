using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    internal static class ParameterInfoExtension
    {
        public string
    }
}
