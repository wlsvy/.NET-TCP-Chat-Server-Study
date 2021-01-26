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
            const string contentsGroup = "ContentsGroup";
            const string name = "name";
            const string serverToClient = "ServerToClient";
            const string clientToServer = "ClientToServer";

            foreach (var element in doc.Elements())
            {
                if(element.Name.LocalName != contentsGroup)
                {
                    throw new NotImplementedException();
                }
                var groupName = element.Attribute(name)?.Value;
                switch (groupName)
                {
                    case serverToClient: ParseProtocol(path, element); break;
                    case clientToServer: ParseProtocol(path, element); break;
                    default: throw new NotImplementedException();
                }
            }
        }

        private static void ParseProtocol(string path, XElement protocols)
        {
            foreach (var protocol in protocols.Elements())
            {
                var protocolName = protocol.Attribute("name").Value;
                var paramList = new List<(Type paramType, string paramName)>();
                foreach(var param in protocol.Elements())
                {
                    var paramTypeName = param.Attribute("type").Value;
                    var paramName = param.Attribute("name").Value;
                    paramList.Add((Type.GetType(paramTypeName), paramName));
                }
            }
        }
    }
}
