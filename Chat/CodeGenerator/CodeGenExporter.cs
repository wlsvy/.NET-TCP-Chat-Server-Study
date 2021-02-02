using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CodeGenerator.Helper;

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

            ClearPreviousCodeGenFile();

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

        private static void ClearPreviousCodeGenFile()
        {
            var sharedDirectory = CodeGenUtil.SHARED_DIR_PATH;
            var files = Directory.GetFiles(sharedDirectory, "*.g.cs", SearchOption.AllDirectories);

            foreach(var f in files)
            {
                File.Delete(f);
            }
        }

        private static void Export(CodeGenContext context)
        {
            var path = Path.Combine(context.DirectoryPath, context.FileName);
            using(var writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                writer.Write(context.ToString());
            }
        }
    }
}
