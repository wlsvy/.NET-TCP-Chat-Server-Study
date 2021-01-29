using CodeGenerator.Protocol;
using System;

namespace CodeGenerator.Writer
{
    internal sealed class BlockWriter : IDisposable
    {
        private readonly CodeGenContext m_Context;

        public BlockWriter(CodeGenContext context)
        {
            m_Context = context;

            m_Context.AppendLine("{");
            m_Context.Tap();
        }

        public void Dispose()
        {
            m_Context.UnTap();
            m_Context.AppendLine("}");
        }

        #region STATIC METHOD

        public static BlockWriter Block(CodeGenContext context, string line)
        {
            context.AppendLine(line);
            return new BlockWriter(context);
        }

        public static BlockWriter Enum(CodeGenContext context, AccessModifier accessModifier, string name, string type = null)
        {
            type = string.IsNullOrEmpty(type) ? string.Empty : $": {type}";

            context.AppendLine($"{accessModifier.String()}enum {name} {type}");
            return new BlockWriter(context);
        }

        public static BlockWriter Class(CodeGenContext context, AccessModifier accessModifier, ClassModifier classModifier, string name, params string[] bases)
        {
            string declarationStr = $"{accessModifier.String()}{classModifier.String()}class {name}";

            if (bases.Length > 0)
            {
                for (var i = 0; i < bases.Length; ++i)
                {
                    if (i == 0)
                    {
                        declarationStr += $" : {bases[i]}";
                    }
                    else
                    {
                        declarationStr += $", {bases[i]}";
                    }
                }
            }

            context.AppendLine(declarationStr);
            return new BlockWriter(context);
        }

        public static BlockWriter Constructor(CodeGenContext context, AccessModifier accessModifier, string name, string baseInit, params ProtocolParameter[] parameters)
        {
            if (string.IsNullOrEmpty(baseInit))
            {
                context.AppendLine($"{accessModifier.String()}{name}({ProtocolParameter.Concat(parameters)})");
            }
            else
            {
                context.AppendLine($"{accessModifier.String()}{name}({ProtocolParameter.Concat(parameters)}) : {baseInit}");
            }
            return new BlockWriter(context);
        }

        public static BlockWriter Method(CodeGenContext context, AccessModifier accessModifier, MethodModifier methodModifier, string ret, string name, params ProtocolParameter[] parameters)
        {
            context.AppendLine($"{accessModifier.String()}{methodModifier.String()}{ret} {name}({ProtocolParameter.Concat(parameters)})");
            return new BlockWriter(context);
        }

        public static BlockWriter Method(CodeGenContext context, AccessModifier accessModifier, MethodModifier methodModifier, string ret, string name, params string[] parameters)
        {
            context.AppendLine($"{accessModifier.String()}{methodModifier.String()}{ret} {name}({ProtocolParameter.Concat(parameters)})");
            return new BlockWriter(context);
        }

        public static BlockWriter Interface(CodeGenContext context, AccessModifier accessModifier, string name)
        {
            context.AppendLine($"{accessModifier.String()}interface {name}");
            return new BlockWriter(context);
        }

        public static BlockWriter Namespace(CodeGenContext context, string nameSpace)
        {
            context.AppendLine($"namespace {nameSpace}");
            return new BlockWriter(context);
        }

        public static BlockWriter If(CodeGenContext context, string condition)
        {
            context.AppendLine($"if({condition})");
            return new BlockWriter(context);
        }

        public static BlockWriter Switch(CodeGenContext context, string expression)
        {
            context.AppendLine($"switch ({expression})");
            return new BlockWriter(context);
        }

        public static BlockWriter Case(CodeGenContext context, string comparand)
        {
            context.AppendLine($"case {comparand}:");
            return new BlockWriter(context);
        }

        public static BlockWriter Using(CodeGenContext context, string varName, string typename, string parameters)
        {
            context.AppendLine($"using ({BaseTypes.VAR} {varName} = new {typename}({parameters}))");
            return new BlockWriter(context);
        }



        #endregion
    }
}
