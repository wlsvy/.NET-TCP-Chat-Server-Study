namespace CodeGenerator.Writer
{
    internal static class LineWriter
    {
        public static void UsingNamespace(CodeGenContext context, string nameSpace)
        {
            context.AppendLine($"using {nameSpace};");
        }

        public static void LineSpace(CodeGenContext context)
        {
            context.AppendLine(string.Empty);
        }

        public static void WithComma(CodeGenContext context, string line)
        {
            context.AppendLine($"{line},");
        }
    }
}
