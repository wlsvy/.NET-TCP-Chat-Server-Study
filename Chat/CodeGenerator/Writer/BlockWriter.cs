﻿using CodeGenerator.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static BlockWriter Enum(CodeGenContext context, AccessModifier accessModifier, string name, string type = null)
        {
            _ = name ?? throw new ArgumentNullException(nameof(name));
            type = string.IsNullOrEmpty(type) ? string.Empty : $" : {type}";

            context.AppendLine($"{accessModifier.String()}enum {name} {type}");
            return new BlockWriter(context);
        }

        public static BlockWriter Class(CodeGenContext context, AccessModifier accessModifier, ClassModifier classModifier, string name, params string[] bases)
        {
            _ = name ?? throw new ArgumentNullException(nameof(name));

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

        public static BlockWriter Constructor(CodeGenContext context, AccessModifier accessModifier, string name, CodeGenParam p)
        {
            _ = name ?? throw new ArgumentNullException(nameof(name));



            return new BlockWriter(context);
        }

        public static BlockWriter Method(CodeGenContext context, AccessModifier accessModifier, string name)
        {
            return new BlockWriter(context);
        }

        public static BlockWriter Interface(CodeGenContext context, AccessModifier accessModifier, string name)
        {
            context.AppendLine($"{accessModifier.String()}interface {name}");
            return new BlockWriter(context);
        }





        #endregion
    }
}