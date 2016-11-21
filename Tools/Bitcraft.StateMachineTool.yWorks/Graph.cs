using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Bitcraft.StateMachineTool.Core;

namespace Bitcraft.StateMachineTool.yWorks
{
    internal class GraphStub : IGraph
    {
        public INode InitialNode { get; }
        public string Semantic { get; }
        public INode[] Nodes { get; }
        public ITransition[] Transitions { get; }

        public GraphStub(INode initialNode, string semantic, INode[] nodes, ITransition[] transitions)
        {
            InitialNode = initialNode;
            Semantic = semantic;
            Nodes = nodes;
            Transitions = transitions;
        }

        public override string ToString()
        {
            return Semantic + (InitialNode != null ? " [initial: " + InitialNode.Semantic + "]" : "");
        }
    }

    public class Graph : GraphObject
    {
        public Node InitialNode { get; private set; }
        public Node[] Nodes { get; private set; }
        public Transition[] Transitions { get; private set; }

        public override void Load(XElement element, KeyMapping keyMapping)
        {
            base.Load(element, keyMapping);

            Nodes = element
                .Elements(XName.Get("node", element.GetDefaultNamespace().NamespaceName))
                .Select(x => Node.Create(x, keyMapping))
                .ToArray();

            Transitions = element
                .Elements(XName.Get("edge", element.GetDefaultNamespace().NamespaceName))
                .Select(x => Transition.Create(x, keyMapping))
                .Distinct(new TransitionEqualityComparer())
                .ToArray();

            var initialNodes = Nodes
                .Where(n => n.IsInitial)
                .ToArray();

            if (initialNodes.Length == 1)
                InitialNode = initialNodes[0];
            else if (initialNodes.Length > 1)
                throw new FormatException("Several nodes are declared as initial, however there can be at most one.");

            // find not properly connected transitions
            foreach (var t in Transitions)
            {
                if (Nodes.Any(n => n.Identifier == t.Source) == false)
                    throw new FormatException(string.Format("Source of transition '{0}' is invalid.", t.Description));
                if (Nodes.Any(n => n.Identifier == t.Target) == false)
                    throw new FormatException(string.Format("Target of transition '{0}' is invalid.", t.Description));

                if (t.Description == null)
                    t.UpdateDescription(Nodes.First(n => n.Identifier == t.Target).Description);
            }

            //foreach (var grp in Transitions.GroupBy(t => t.Description))
            //{
            //    var orderedTransitions = grp.OrderBy(t => t.Source).ToArray();
            //    for (int i = 0; i < orderedTransitions.Length - 1; i++)
            //    {
            //        if (orderedTransitions[i].Source == orderedTransitions[i + 1].Source)
            //        {
            //            var from = Nodes.Single(x => x.Identifier == orderedTransitions[i].Source).Description;
            //            var to1 = Nodes.Single(x => x.Identifier == orderedTransitions[i].Target).Description;
            //            var to2 = Nodes.Single(x => x.Identifier == orderedTransitions[i + 1].Target).Description;
            //            throw new FormatException(string.Format("Illegal transitions from '{0}' to '{1}' and '{2}'.", from, to1, to2));
            //        }
            //    }
            //}
        }

        public static Graph Create(XElement element, KeyMapping keyMapping)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (keyMapping == null)
                throw new ArgumentNullException(nameof(keyMapping));

            var graph = new Graph();
            graph.Load(element, keyMapping);
            return graph;
        }

        public static Graph CreateFromRootXml(XElement root)
        {
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            var km = new KeyMapping(root);

            var graphElement = root
                .Elements(XName.Get("graph", root.GetDefaultNamespace().NamespaceName))
                .FirstOrDefault();

            return Create(graphElement, km);
        }
    }
}
