using CodeGenerator.Protocol;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeGenerator
{
    internal static class Program
    {
        public const string ROOT_DIRECTORY_NAME = ".NET-TCP-Chat-Server-Study";

        static void Main(string[] args)
        {
            Global.Initialize();

            var docs = LoadXml("Schema");
            var protocolsContents = ProtocolContentParser.Parse(docs);
        }


        private static IReadOnlyCollection<(string, XDocument)> LoadXml(string path)
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
