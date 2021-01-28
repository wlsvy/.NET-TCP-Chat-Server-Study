using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    internal sealed class CodeGenContext
    {
        public readonly string DirectoryPath;
        public readonly string FileName;

        private readonly StringBuilder m_StringBuilder = new StringBuilder();
        private uint m_Tap = 0;

        public CodeGenContext(string directoryPath, string fileName)
        {
            DirectoryPath = directoryPath;
            FileName = fileName;
        }

        public void Tap() => m_Tap++;
        public void UnTap()
        {
            if(m_Tap <= 0)
            {
                throw new InvalidOperationException("더 이상 Untap을 할 수 없음");
            }
            m_Tap--;
        }


        public void Append(string value)
        {
            m_StringBuilder.Append(value);
        }
        public void AppendLine(string line)
        {
            for(int i = 0; i < m_Tap; i++)
            {
                m_StringBuilder.Append("    ");
            }
            m_StringBuilder.AppendLine(line);
        }

        public override string ToString()
        {
            return m_StringBuilder.ToString();
        }
    }
}
