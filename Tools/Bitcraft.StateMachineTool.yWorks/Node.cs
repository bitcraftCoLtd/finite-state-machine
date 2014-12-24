using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Bitcraft.StateMachineTool.Core;

namespace Bitcraft.StateMachineTool.yWorks
{
    public class NodeStub : INode
    {
        public bool IsFinal { get; private set; }
        public string Semantic { get; private set; }

        public NodeStub(string semantic)
            : this(semantic, false)
        {
        }

        public NodeStub(string semantic, bool isFinal)
        {
            Semantic = semantic;
            IsFinal = isFinal;
        }

        public override string ToString()
        {
            return Semantic + (IsFinal ? " [final]" : "");
        }
    }

    public class Node : GraphObject
    {
        public bool IsInitial { get; private set; }
        public bool IsFinal { get; internal set; }

        public override void Load(XElement element, KeyMapping keyMapping)
        {
            base.Load(element, keyMapping);

            var dataElements = element
                .Elements(XName.Get("data", element.GetDefaultNamespace().NamespaceName))
                .ToArray();

            var initialElem = dataElements
                .Where(x => (string)x.Attribute("key") == keyMapping.InitialStateId)
                .FirstOrDefault();

            var finalElem = dataElements
                .Where(x => (string)x.Attribute("key") == keyMapping.FinalStateId)
                .FirstOrDefault();

            IsInitial = ParsingUtility.ElementContentToBoolean(initialElem, keyMapping.InitialStateDefaultValue);
            IsFinal = ParsingUtility.ElementContentToBoolean(finalElem, keyMapping.FinalStateDefaultValue);

            if (IsFinal)
            {
                if (Description == null)
                    Description = "[FINAL]";
            }

            CheckIdProperty(element);
            CheckDescriptionProperty(element);
        }

        public static Node Create(XElement element, KeyMapping keyMapping)
        {
            var node = new Node();
            node.Load(element, keyMapping);
            return node;
        }

        public override string ToString()
        {
            string flags = string.Empty;

            if (IsInitial && IsFinal)
                flags = " [initial, final]";
            else if (IsInitial)
                flags = " [initial]";
            else if (IsFinal)
                flags = " [final]";

            return base.ToString() + flags;
        }
    }
}
