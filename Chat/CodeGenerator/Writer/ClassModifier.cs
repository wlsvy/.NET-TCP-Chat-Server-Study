using System;

namespace CodeGenerator.Writer
{
    internal enum ClassModifier : byte
    {
        Empty,
        Abstract,
        Sealed,
        Static,
    }

    internal static class ClassModifierExtension
    {
        public static string String(this ClassModifier classModifier) => classModifier switch
        {
            ClassModifier.Empty => " ",
            ClassModifier.Abstract => "abstract ",
            ClassModifier.Sealed => "sealed ",
            ClassModifier.Static => "static ",
            _ => throw new ArgumentOutOfRangeException(nameof(classModifier)),
        };
    }
}
