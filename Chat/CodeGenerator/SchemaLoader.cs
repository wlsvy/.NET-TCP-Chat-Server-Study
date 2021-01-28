using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeGenerator
{
    internal static class SchemaLoader
    {
        public static IReadOnlyCollection<(string, XDocument)> LoadXmlSchema(string path)
        {
            var docs = new ConcurrentQueue<(string, XDocument)>();
            var paths = Directory.GetFiles(path, "*.xml", SearchOption.AllDirectories);
            var tasks = new List<Task>(paths.Length);

            foreach (var p in paths)
            {
                tasks.Add(Task.Run(() =>
                {
                    var doc = XDocument.Load(p, LoadOptions.SetLineInfo);
                    docs.Enqueue((p, doc));
                }));
            }
            Task.WaitAll(tasks.ToArray());
            return docs.ToArray();
        }
    }
}
