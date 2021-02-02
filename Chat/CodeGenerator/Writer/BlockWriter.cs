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

            m_Context.Line("{");
            m_Context.Tap();
        }

        public void Dispose()
        {
            m_Context.UnTap();
            m_Context.Line("}");
        }

        #region STATIC METHOD

        public static BlockWriter Block(CodeGenContext context, string line)
        {
            context.Line(line);
            return new BlockWriter(context);
        }

        public static BlockWriter Enum(CodeGenContext context, AccessModifier accessModifier, string name, string type = null)
        {
            type = string.IsNullOrEmpty(type) ? string.Empty : $": {type}";

            context.Line($"{accessModifier.String()}enum {name} {type}");
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

            context.Line(declarationStr);
            return new BlockWriter(context);
        }

        public static BlockWriter Constructor(CodeGenContext context, AccessModifier accessModifier, string name, string baseInit, params ProtocolParameter[] parameters)
        {
            if (string.IsNullOrEmpty(baseInit))
            {
                context.Line($"{accessModifier.String()}{name}({ProtocolParameter.Concat(parameters)})");
            }
            else
            {
                context.Line($"{accessModifier.String()}{name}({ProtocolParameter.Concat(parameters)}) : {baseInit}");
            }
            return new BlockWriter(context);
        }

        public static BlockWriter Method(CodeGenContext context, AccessModifier accessModifier, MethodModifier methodModifier, string ret, string name, params ProtocolParameter[] parameters)
        {
            context.Line($"{accessModifier.String()}{methodModifier.String()}{ret} {name}({ProtocolParameter.Concat(parameters)})");
            return new BlockWriter(context);
        }

        public static BlockWriter Method(CodeGenContext context, AccessModifier accessModifier, MethodModifier methodModifier, string ret, string name, params string[] parameters)
        {
            context.Line($"{accessModifier.String()}{methodModifier.String()}{ret} {name}({ProtocolParameter.Concat(parameters)})");
            return new BlockWriter(context);
        }

        public static BlockWriter Interface(CodeGenContext context, AccessModifier accessModifier, string name)
        {
            context.Line($"{accessModifier.String()}interface {name}");
            return new BlockWriter(context);
        }

        public static BlockWriter Namespace(CodeGenContext context, string nameSpace)
        {
            context.Line($"namespace {nameSpace}");
            return new BlockWriter(context);
        }

        public static BlockWriter If(CodeGenContext context, string condition)
        {
            context.Line($"if({condition})");
            return new BlockWriter(context);
        }

        public static BlockWriter Switch(CodeGenContext context, string expression)
        {
            context.Line($"switch ({expression})");
            return new BlockWriter(context);
        }

        public static BlockWriter Case(CodeGenContext context, string comparand)
        {
            context.Line($"case {comparand}:");
            return new BlockWriter(context);
        }

        public static BlockWriter Using(CodeGenContext context, string varName, string typename, string parameters)
        {
            context.Line($"using ({BaseTypes.VAR} {varName} = new {typename}({parameters}))");
            return new BlockWriter(context);
        }



        #endregion
    }
}
