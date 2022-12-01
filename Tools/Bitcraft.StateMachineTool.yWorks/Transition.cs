using System.Xml.Linq;
using Bitcraft.StateMachineTool.Core;

namespace Bitcraft.StateMachineTool.yWorks;

internal class TransitionStub : ITransition
{
    public string Semantic { get; }
    public INode Source { get; }
    public INode Target { get; }

    public TransitionStub(string semantic, INode source, INode target)
    {
        Semantic = semantic;
        Source = source;
        Target = target;
    }

    public override string ToString()
    {
        return $"{Source.Semantic} -> {Target.Semantic}";
    }
}

public class Transition : GraphObject
{
    public string? Source { get; }
    public string? Target { get; }

    protected Transition(string identifier, string description, string? source, string? target)
        : base(identifier, description)
    {
        Source = source;
        Target = target;
    }

    public static new Transition Load(XElement element, KeyMapping keyMapping)
    {
        GraphObject graphObject = GraphObject.Load(element, keyMapping);

        CheckIdProperty(element, graphObject);

        string? source = element.Attribute("source")?.Value;
        string? target = element.Attribute("target")?.Value;

        if (source != null)
            source = source.Trim();

        if (target != null)
            target = target.Trim();

        return new Transition(graphObject.Identifier, graphObject.Description, source, target);
    }

    internal void UpdateDescription(string newDescription)
    {
        Description = newDescription;
    }

    public override string ToString()
    {
        return base.ToString() + " { " + string.Format("{0} -> {1}", Source, Target) + " }";
    }
}
