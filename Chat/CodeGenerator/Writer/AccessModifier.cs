using System;

namespace CodeGenerator.Writer
{
    public enum AccessModifier : byte
    {
        Public,
        Private,
        Protected,
        Internal,
        Proteced_Internal,
        Private_Protected,
    }

    internal static class AccessModifierExtension
    {
        public static string String(this AccessModifier accessModifier) => accessModifier switch
        {
            AccessModifier.Public => "public",
            AccessModifier.Private => "private",
            AccessModifier.Protected => "protected",
            AccessModifier.Internal => "internal",
            AccessModifier.Private_Protected => "private internal",
            AccessModifier.Proteced_Internal => "protected internal",
            _ => throw new ArgumentOutOfRangeException(nameof(accessModifier)),
        };
    }
}
