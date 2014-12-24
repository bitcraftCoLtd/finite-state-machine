using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.StateMachineTool.Core
{
    public interface IGraph : IGraphElement
    {
        INode InitialNode { get; }
        INode[] Nodes { get; }
        ITransition[] Transitions { get; }
    }
}
