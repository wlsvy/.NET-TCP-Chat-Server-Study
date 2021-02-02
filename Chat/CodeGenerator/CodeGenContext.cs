﻿using System;
using System.Text;

namespace CodeGenerator
{
    internal sealed class CodeGenContext
    {
        public sealed class CodeScope : IDisposable
        {
            private readonly CodeGenContext m_Context;

            public CodeScope(CodeGenContext context)
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
        }

        public readonly string DirectoryPath;
        public readonly string FileName;

        private readonly StringBuilder m_StringBuilder = new StringBuilder();
        private uint m_Tap = 0;
        public int Length => m_StringBuilder.Length;

        public CodeGenContext(string directoryPath, string fileName)
        {
            DirectoryPath = directoryPath;
            FileName = fileName;

            Line("//======================================");
            Line("//======       AutoGenerated       =====");
            Line("//======================================");
            LineSpace();
        }

        public void Line(string line)
        {
            for(int i = 0; i < m_Tap; i++)
            {
                m_StringBuilder.Append("    ");
            }
            m_StringBuilder.AppendLine(line);
        }

        public void LineSpace()
        {
            m_StringBuilder.AppendLine();
        }

        public CodeScope Scope(string line)
        {
            Line(line);
            return new CodeScope(this);
        }

        public override string ToString()
        {
            return m_StringBuilder.ToString();
        }

        private void Tap() => m_Tap++;
        private void UnTap()
        {
            if (m_Tap <= 0)
            {
                throw new InvalidOperationException("더 이상 Untap을 할 수 없음");
            }
            m_Tap--;
        }
    }
}
