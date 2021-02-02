﻿using CodeGenerator.Protocol;

namespace CodeGenerator.Writer
{
    internal static class LineWriter
    {
        public static void CodeGenCaption(CodeGenContext context)
        {
            context.Line("//======================================");
            context.Line("//======       AutoGenerated       =====");
            context.Line("//======================================");
        }

        public static void Line(CodeGenContext context, string line)
        {
            context.Line(line);
        }

        public static void LineSpace(CodeGenContext context)
        {
            context.Line(string.Empty);
        }

        public static void AppendPreviousLine(CodeGenContext context, string line)
        {
            context.Insert(context.Length - 2, line);
        }

        public static void UsingNamespace(CodeGenContext context, string nameSpace)
        {
            context.Line($"using {nameSpace};");
        }

        public static void InterfaceMethod(CodeGenContext context, string returnType, string name, params CodeGenParam[] parameters)
        {
            context.Line($"{returnType} {name}({CodeGenParam.Concat(parameters)});");
        }

        public static void Field(CodeGenContext context, AccessModifier accessModifier, FieldModifier fieldModifier, string typename, string name, string defaultValue = null)
        {
            defaultValue = string.IsNullOrEmpty(defaultValue)
                ? string.Empty
                : $" = {defaultValue}";
            context.Line($"{accessModifier.String()}{fieldModifier.String()}{typename} {name}{defaultValue};");
        }

        public static void LocalVariable(CodeGenContext context, string typename, string name, string value = null)
        {
            typename = string.IsNullOrEmpty(typename)
                ? $"{BaseTypes.VAR}"
                : typename;
            value = string.IsNullOrEmpty(value)
                ? string.Empty
                : $" = {value}";

            context.Line($"{typename} {name}{value};");
        }

        public static void Return(CodeGenContext context, string value = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                context.Line("return;");
            }
            else
            {
                context.Line($"return {value};");
            }
        }

        public static void Break(CodeGenContext context)
        {
            context.Line("break;");
        }

        public static void Case_Break(CodeGenContext context, string comparand, string line)
        {
            context.Line($"case {comparand}: {line} break;");
        }

        public static void Case_Return(CodeGenContext context, string comparand, string line)
        {
            context.Line($"case {comparand}: {line} return;");
        }
    }
}
