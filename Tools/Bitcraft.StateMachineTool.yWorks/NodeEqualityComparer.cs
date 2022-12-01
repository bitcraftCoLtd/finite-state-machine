using Bitcraft.StateMachineTool.Core;

namespace Bitcraft.StateMachineTool.yWorks;

public class NodeEqualityComparer : IEqualityComparer<INode>
{
    public bool Equals(INode? x, INode? y)
    {
        if (x == null || y == null)
            return false;

        return x.Semantic == y.Semantic;
    }

    public int GetHashCode(INode? obj)
    {
        if (obj == null || obj.Semantic == null)
            return 0;

        return obj.Semantic.GetHashCode();
    }
}
