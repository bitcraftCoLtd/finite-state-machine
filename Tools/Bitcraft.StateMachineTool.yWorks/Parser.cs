using System.Xml.Linq;
using Bitcraft.StateMachineTool.Core;

namespace Bitcraft.StateMachineTool.yWorks;

public class Parser : IParser
{
    public IGraph Parse(Stream stream)
    {
        var graph = Graph.CreateFromRootXml(XElement.Load(stream, LoadOptions.SetLineInfo));

        CheckNodesMergeability(graph.Nodes);

        var nodes = graph.Nodes
            .Where(n => n.Description != null)
            .Select(n => (INode)new NodeStub(n.Description!, n.IsFinal))
            .Distinct(new NodeEqualityComparer())
            .ToList();

        int i = 0;
        var transitions = new ITransition[graph.Transitions.Count];

        foreach (Transition tr in graph.Transitions)
        {
            Node sourceState = graph.Nodes.Single(n => n.Identifier == tr.Source);
            Node targetState = graph.Nodes.Single(n => n.Identifier == tr.Target);

            INode sourceNode = nodes.Single(n => n.Semantic == sourceState.Description);
            INode targetNode = nodes.Single(n => n.Semantic == targetState.Description);

            transitions[i++] = new TransitionStub(tr.Description, sourceNode, targetNode);
        }

        INode? initialNode = null;

        if (graph.InitialNode != null)
        {
            int index = -1;
            int initialNodeIndex = -1;

            foreach (Node node in graph.Nodes)
            {
                index++;
                if (node.Identifier == graph.InitialNode.Identifier)
                {
                    initialNodeIndex = index;
                    break;
                }
            }

            initialNode = nodes[initialNodeIndex];
        }

        return new GraphStub(initialNode, graph.Description, nodes, transitions);
    }

    private static void CheckNodesMergeability(IEnumerable<Node> nodes)
    {
        foreach (var group in nodes.GroupBy(n => n.Description))
        {
            if (group == null)
                continue;

            CheckPropertyMergeability(Constants.IsInitialStatePropertyName, n => n?.IsInitial ?? false, group);
            CheckPropertyMergeability(Constants.IsFinalStatePropertyName, n => n?.IsFinal ?? false, group);
        }
    }

    private static void CheckPropertyMergeability<T>(string propertyName, Func<Node?, T?> getter, IGrouping<string?, Node?> group)
    {
        T? value = getter(group.First());

        if (group.Any(n => EqualityComparer<T>.Default.Equals(getter(n), value) == false))
            throw new FormatException($"Inconsistency in partial state '{group.Key}'. Property '{propertyName}' does not match on all states.");
    }
}
