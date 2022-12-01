using System.Xml.Linq;
using Bitcraft.StateMachineTool.Core;

namespace Bitcraft.StateMachineTool.yWorks;

public class NodeStub : INode
{
    public string Semantic { get; }
    public bool IsFinal { get; }

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
        return $"{Semantic}{(IsFinal ? " [final]" : string.Empty)}";
    }
}

public class Node : GraphObject
{
    public bool IsInitial { get; }
    public bool IsFinal { get; }

    protected Node(string identifier, string description, bool isInitial, bool isFinal)
        : base(identifier, description)
    {
        IsInitial = isInitial;
        IsFinal = isFinal;
    }

    public static new Node Load(XElement element, KeyMapping keyMapping)
    {
        GraphObject graphObject = GraphObject.Load(element, keyMapping);

        var dataElements = element
            .Elements(XName.Get("data", element.GetDefaultNamespace().NamespaceName))
            .ToArray();

        XElement? initialElem = dataElements
            .Where(x => x.Attribute("key")?.Value == keyMapping.InitialStateId)
            .FirstOrDefault();

        XElement? finalElem = dataElements
            .Where(x => x.Attribute("key")?.Value == keyMapping.FinalStateId)
            .FirstOrDefault();

        bool isInitial = ParsingUtility.ElementContentToBoolean(initialElem, keyMapping.InitialStateDefaultValue);
        bool isFinal = ParsingUtility.ElementContentToBoolean(finalElem, keyMapping.FinalStateDefaultValue);

        string? description = graphObject.Description;

        if (isFinal)
            description ??= "[FINAL]";

        CheckIdProperty(element, graphObject);
        CheckDescriptionProperty(element, graphObject);

        return new Node(graphObject.Identifier, graphObject.Description, isInitial, isFinal);
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
