using CodeGenerator.Helper;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CodeGenerator.Protocol
{
    internal static class ProtocolContentParser
    {
        public static IReadOnlyList<ProtocolContent> Parse(IReadOnlyCollection<(string path, XDocument doc)> list)
        {
            var protocolContents = new List<ProtocolContent>();

            foreach (var (path, doc) in list)
            {
                var contents = Parse(path, doc);
                protocolContents.AddRange(contents);
            }
            return protocolContents;
        }

        public static IReadOnlyList<ProtocolContent> Parse(string path, XDocument doc)
        {
            const string contentsGroup = "ContentsGroup";
            var protocolContents = new List<ProtocolContent>();

            foreach (var element in doc.Elements())
            {
                if (element.Name.LocalName != contentsGroup)
                {
                    throw new NotImplementedException();
                }

                var groupName = element.Attribute(ContentsGroupAttribue.name.ToString())?.Value;
                if(groupName != ContentsGroupType.Protocol.ToString())
                {
                    throw new NotImplementedException();
                }
                protocolContents.AddRange(ParseProtocol(element));
            }
            return protocolContents;
        }

        private static IReadOnlyList<ProtocolContent> ParseProtocol(XElement protocols)
        {
            var protocolContents = new List<ProtocolContent>();
            foreach (var protocol in protocols.Elements())
            {
                var protocolName = protocol.Attribute(ProtocolContent.ProtocolAttribute.name.ToString()).Value;
                var direction = protocol.Attribute(ProtocolContent.ProtocolAttribute.direction.ToString()).Value;
                var paramList = new List<CodeGenParam>();
                foreach (var param in protocol.Elements())
                {
                    var paramTypeName = param.Attribute(ProtocolContent.ParameterAttribute.type.ToString()).Value;
                    var paramName = param.Attribute(ProtocolContent.ParameterAttribute.name.ToString()).Value;
                    paramList.Add(new CodeGenParam(paramTypeName, paramName));
                }
                protocolContents.Add(new ProtocolContent(protocolName, direction, paramList.ToArray()));
            }
            return protocolContents;
        }
    }
}
