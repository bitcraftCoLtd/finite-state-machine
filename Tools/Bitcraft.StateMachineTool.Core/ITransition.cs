namespace Bitcraft.StateMachineTool.Core;

public interface ITransition : IGraphElement
{
    INode Source { get; }
    INode Target { get; }
}
