namespace Bitcraft.StateMachineTool.Core;

public interface INode : IGraphElement
{
    bool IsFinal { get; }
}
