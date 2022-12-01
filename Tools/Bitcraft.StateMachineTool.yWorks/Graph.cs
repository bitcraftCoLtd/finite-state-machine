using System.Collections.ObjectModel;
using System.Xml.Linq;
using Bitcraft.StateMachineTool.Core;

namespace Bitcraft.StateMachineTool.yWorks;

internal class GraphStub : IGraph
{
    public INode? InitialNode { get; }
    public string Semantic { get; }
    public IReadOnlyCollection<INode> Nodes { get; }
    public IReadOnlyCollection<ITransition> Transitions { get; }

    public GraphStub(INode? initialNode, string semantic, IReadOnlyCollection<INode> nodes, IReadOnlyCollection<ITransition> transitions)
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
    public Node? InitialNode { get; }
    public IReadOnlyCollection<Node> Nodes { get; }
    public IReadOnlyCollection<Transition> Transitions { get; }

    protected Graph(string identifier, string description, Node? initialNode, IReadOnlyCollection<Node> nodes, IReadOnlyCollection<Transition> transitions)
        : base(identifier, description)
    {
        InitialNode = initialNode;
        Nodes = nodes;
        Transitions = transitions;
    }

    public static new Graph Load(XElement element, KeyMapping keyMapping)
    {
        GraphObject? graphObject = GraphObject.Load(element, keyMapping);

        if (graphObject == null)
            throw new InvalidDataException();

        List<Node> nodes = element
            .Elements(XName.Get("node", element.GetDefaultNamespace().NamespaceName))
            .Select(x => Node.Load(x, keyMapping))
            .ToList();

        List<Transition> transitions = element
            .Elements(XName.Get("edge", element.GetDefaultNamespace().NamespaceName))
            .Select(x => Transition.Load(x, keyMapping))
            .Distinct(new TransitionEqualityComparer())
            .ToList();

        var initialNodes = nodes
            .Where(n => n.IsInitial)
            .ToList();

        Node? initialNode = null;

        if (initialNodes.Count == 1)
            initialNode = initialNodes[0];
        else if (initialNodes.Count > 1)
            throw new FormatException("Several nodes are declared as initial, however there can be at most one.");

        // Find not properly connected transitions.
        foreach (var t in transitions)
        {
            if (nodes.Any(n => n.Identifier == t.Source) == false)
                throw new FormatException(string.Format("Source of transition '{0}' is invalid.", t.Description));
            if (nodes.Any(n => n.Identifier == t.Target) == false)
                throw new FormatException(string.Format("Target of transition '{0}' is invalid.", t.Description));

            if (t.Description == null)
                t.UpdateDescription(nodes.First(n => n.Identifier == t.Target).Description);
        }

        return new Graph(
            graphObject.Identifier,
            graphObject.Description,
            initialNode,
            new ReadOnlyCollection<Node>(nodes),
            new ReadOnlyCollection<Transition>(transitions)
        );
    }

    public static Graph CreateFromRootXml(XElement? root)
    {
        if (root == null)
            throw new ArgumentNullException(nameof(root));

        var km = new KeyMapping(root);

        XElement? graphElement = root
            .Elements(XName.Get("graph", root.GetDefaultNamespace().NamespaceName))
            .FirstOrDefault();

        if (graphElement == null)
            throw new InvalidDataException("Could not find 'graph' node in input data.");

        return Load(graphElement, km);
    }
}
