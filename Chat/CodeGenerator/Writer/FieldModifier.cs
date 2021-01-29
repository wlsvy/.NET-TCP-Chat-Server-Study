using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Writer
{
    internal enum FieldModifier
    {
        Empty,
        Readonly,
        Static,
        Const,
        Volatile
    }

    internal static class FieldModifierExtension
    {
        public static string String(this FieldModifier fieldModifier) => fieldModifier switch
        {
            FieldModifier.Empty => string.Empty,
            FieldModifier.Readonly => "readonly ",
            FieldModifier.Static => "static ",
            FieldModifier.Const => "const ",
            FieldModifier.Volatile => "volatile ",
            _ => throw new ArgumentOutOfRangeException(nameof(fieldModifier)),
        };
    }
}
