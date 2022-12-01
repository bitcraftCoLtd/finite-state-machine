namespace Bitcraft.StateMachineTool.Core;

public interface IGraph : IGraphElement
{
    INode? InitialNode { get; }
    IReadOnlyCollection<INode> Nodes { get; }
    IReadOnlyCollection<ITransition> Transitions { get; }
}
