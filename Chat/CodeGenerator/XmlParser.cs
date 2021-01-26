using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CodeGenerator
{
    internal static class XmlParser
    {
        public static void ToNetworkMessage(IReadOnlyCollection<(string path, XDocument doc)> list)
        {
            foreach(var (path, doc) in list)
            {
                ToNetworkMessage(path, doc);
            }
        }

        public static void ToNetworkMessage(string path, XDocument doc)
        {
           foreach(var element in doc.Elements())
            {
                switch (element.Name.LocalName)
                {
                    case "ServerToClient": break;
                    case "ClientToServer": break;
                }
            }
        }
    }
}
