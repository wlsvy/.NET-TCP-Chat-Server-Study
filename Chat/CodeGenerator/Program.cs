using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var docs = LoadXml();

        }

        private static IReadOnlyCollection<(string, XDocument)> LoadXml()
        {
            var docs = new ConcurrentQueue<(string, XDocument)>();

            var tasks = new List<Task>();
            foreach (var path in Directory.GetFiles("Schema", "*.xml", SearchOption.AllDirectories))
            {
                tasks.Add(Task.Run(() =>
                {
                    var doc = XDocument.Load(path, LoadOptions.SetLineInfo);
                    var item = (path, doc);
                    docs.Enqueue(item);
                }));
            }
            Task.WaitAll(tasks.ToArray());
            return docs.ToArray();
        }
    }
}
