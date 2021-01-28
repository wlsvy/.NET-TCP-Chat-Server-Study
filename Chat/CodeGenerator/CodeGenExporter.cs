using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    internal static class CodeGenExporter
    {
        public static void Export(IReadOnlyList<CodeGenContext> contexts)
        {
            foreach(var context in contexts)
            {
                if (!Directory.Exists(context.DirectoryPath))
                {
                    Directory.CreateDirectory(context.DirectoryPath);
                }
            }

            var tasks = new List<Task>(contexts.Count);
            foreach(var context in contexts)
            {
                tasks.Add(Task.Run(() =>
                {
                    Export(context);
                }));
            }
            Task.WaitAll(tasks.ToArray());
        }

        public static void Export(CodeGenContext context)
        {
            var path = Path.Combine(context.DirectoryPath, context.FileName);
            using(var writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                writer.Write(context.ToString());
            }
        }
    }
}
