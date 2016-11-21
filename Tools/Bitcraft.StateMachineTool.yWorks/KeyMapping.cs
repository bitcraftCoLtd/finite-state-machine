using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Bitcraft.StateMachineTool.yWorks
{
    public struct Key
    {
        public bool IsValid { get { return Id != null; } }

        public readonly int Line;
        public readonly int Position;

        public readonly string Id;
        public readonly string AttributeName;
        public readonly string AttributeType;
        public readonly string FileType;
        public readonly string For;
        public readonly string DefaultValue;

        public Key(int line, int pos, string id, string attrName, string attrType, string fileType, string forValue, string defaultValue)
        {
            Line = line;
            Position = pos;

            Id = id != null ? id.ToLowerInvariant() : null;
            AttributeName = attrName != null ? attrName.ToLowerInvariant() : null;
            AttributeType = attrType != null ? attrType.ToLowerInvariant() : null;
            FileType = fileType != null ? fileType.ToLowerInvariant() : null;
            For = forValue != null ? forValue.ToLowerInvariant() : null;

            DefaultValue = defaultValue;
        }
    }

    public class KeyMapping : IReadOnlyCollection<Key>
    {
        private Key[] keys;

        public string GraphDescrionId { get; }
        public string NodeDescrionId { get; }
        public string EdgeDescrionId { get; }

        public string NodeContentId { get; }
        public string EdgeContentId { get; }

        public string InitialStateId { get; }
        public bool InitialStateDefaultValue { get; }

        public string FinalStateId { get; }
        public bool FinalStateDefaultValue { get; }

        public KeyMapping(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (element.Name.LocalName != "graphml")
                throw new ArgumentException("Impossible to find 'graphml' element.");

            var keyElements = element.Elements(XName.Get("key", element.GetDefaultNamespace().NamespaceName));

            var keyList = new List<Key>();

            foreach (var key in keyElements)
            {
                var id = (string)key.Attribute("id");
                var attrName = (string)key.Attribute("attr.name");
                var attrType = (string)key.Attribute("attr.type");
                var fileType = (string)key.Attribute("yfiles.type");
                var forValue = (string)key.Attribute("for");

                string defaultValue = null;

                var defaultElement = key.Element(XName.Get("default", element.GetDefaultNamespace().NamespaceName));
                if (defaultElement != null && defaultElement.FirstNode != null)
                {
                    if (defaultElement.FirstNode.NodeType == XmlNodeType.CDATA)
                        defaultValue = ((XCData)defaultElement.FirstNode).Value;
                    else if (defaultElement.FirstNode.NodeType == XmlNodeType.Text)
                        defaultValue = ((XText)defaultElement.FirstNode).Value;
                }

                keyList.Add(new Key(((IXmlLineInfo)key).LineNumber, ((IXmlLineInfo)key).LinePosition, id, attrName, attrType, fileType, forValue, defaultValue));
            }

            keys = keyList.ToArray();

            GraphDescrionId = keys.FirstOrDefault(k => k.For == "graph" && k.AttributeName == "description" && k.AttributeType == "string").Id;
            NodeDescrionId = keys.FirstOrDefault(k => k.For == "node" && k.AttributeName == "description" && k.AttributeType == "string").Id;
            EdgeDescrionId = keys.FirstOrDefault(k => k.For == "edge" && k.AttributeName == "description" && k.AttributeType == "string").Id;

            NodeContentId = keys.FirstOrDefault(k => k.For == "node" && k.FileType == "nodegraphics").Id;
            EdgeContentId = keys.FirstOrDefault(k => k.For == "edge" && k.FileType == "edgegraphics").Id;

            var initialKey = keys.FirstOrDefault(k => k.For == "node" && k.AttributeName == Constants.IsInitialStatePropertyName.ToLowerInvariant() && k.AttributeType == "int");
            if (initialKey.IsValid)
            {
                InitialStateId = initialKey.Id;
                int value = 0;
                if (initialKey.DefaultValue != null)
                {
                    if (int.TryParse(initialKey.DefaultValue, out value) == false)
                        throw new FormatException(string.Format("Invalid default value at line {0} position {1}.", initialKey.Line, initialKey.Position));
                }
                InitialStateDefaultValue = value != 0;
            }

            var finalKey = keys.FirstOrDefault(k => k.For == "node" && k.AttributeName == Constants.IsFinalStatePropertyName.ToLowerInvariant() && k.AttributeType == "int");
            if (finalKey.IsValid)
            {
                FinalStateId = finalKey.Id;
                int value = 0;
                if (finalKey.DefaultValue != null)
                {
                    if (int.TryParse(finalKey.DefaultValue, out value) == false)
                        throw new FormatException(string.Format("Invalid default value at line {0} position {1}.", finalKey.Line, finalKey.Position));
                }
                FinalStateDefaultValue = value != 0;
            }
        }

        public int Count
        {
            get { return keys.Length; }
        }

        public IEnumerator<Key> GetEnumerator()
        {
            return (IEnumerator<Key>)keys.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return keys.GetEnumerator();
        }
    }
}
