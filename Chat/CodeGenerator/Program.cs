using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using CodeGenerator.Protocol;

namespace CodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var docs = LoadXml();
            var protocolsContents = ProtocolContentParser.Parse(docs);
            
        }

        private static IReadOnlyCollection<(string, XDocument)> LoadXml()
        {
            var docs = new ConcurrentQueue<(string, XDocument)>();
            var paths = Directory.GetFiles("Schema", "*.xml", SearchOption.AllDirectories);
            var tasks = new List<Task>(paths.Length);

            foreach (var path in paths)
            {
                tasks.Add(Task.Run(() =>
                {
                    var doc = XDocument.Load(path, LoadOptions.SetLineInfo);
                    docs.Enqueue((path, doc));
                }));
            }
            Task.WaitAll(tasks.ToArray());
            return docs.ToArray();
        }
    }
}
