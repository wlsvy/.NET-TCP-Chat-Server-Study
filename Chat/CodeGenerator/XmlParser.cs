using System;
using System.Collections.Generic;
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
            foreach (var element in doc.Elements())
            {
                switch (element.Name.LocalName)
                {
                    case "ServerToClient": ParseProtocol(path, element); break;
                    case "ClientToServer": ParseProtocol(path, element); break;
                    default: throw new NotImplementedException();
                }
            }
        }

        private static void ParseProtocol(string path, XElement protocols)
        {
            //var content = new ProtocolContent
            var elements = protocols.Elements();
            foreach (var protocol in protocols.Elements())
            {
                var paramList = new List<(Type paramType, string paramName)>();
                foreach(var param in protocol.Elements())
                {
                    var typeName = param.Name.NamespaceName;
                    var paramName = param.Name.LocalName;
                    paramList.Add((Type.GetType(param.Name.NamespaceName), param.Name.LocalName));
                }
            }
        }
    }
}
