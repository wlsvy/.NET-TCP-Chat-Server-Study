using System;

namespace CodeGenerator.Writer
{
    internal enum MethodModifier : byte
    {
        Empty,
        Abstract,
        Override,
        New,
        Static,
        Sealed
    }

    internal static class MethodModifierExtension
    {
        public static string String(this MethodModifier methodModifier) => methodModifier switch
        {
            MethodModifier.Empty => string.Empty,
            MethodModifier.Abstract => "abstract ",
            MethodModifier.Override => "sealed ",
            MethodModifier.New => "new ",
            MethodModifier.Sealed => "sealed ",
            MethodModifier.Static => "static ",
            _ => throw new ArgumentOutOfRangeException(nameof(methodModifier)),
        };
    }
}
