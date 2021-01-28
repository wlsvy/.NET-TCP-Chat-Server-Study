﻿using CodeGenerator.Helper;

namespace CodeGenerator.Writer
{
    internal static class LineWriter
    {
        public static void CodeGenCaption(CodeGenContext context)
        {
            context.AppendLine("//======================================");
            context.AppendLine("//======       AutoGenerated       =====");
            context.AppendLine("//======================================");
        }

        public static void Line(CodeGenContext context, string line)
        {
            context.AppendLine(line);
        }

        public static void UsingNamespace(CodeGenContext context, string nameSpace)
        {
            context.AppendLine($"using {nameSpace};");
        }

        public static void LineSpace(CodeGenContext context)
        {
            context.AppendLine(string.Empty);
        }

        public static void InterfaceMethod(CodeGenContext context, string returnType, string name, params CodeGenParam[] parameters)
        {
            context.AppendLine($"{returnType} {name}({CodeGenParam.Concat(parameters)});");
        }
    }
}
