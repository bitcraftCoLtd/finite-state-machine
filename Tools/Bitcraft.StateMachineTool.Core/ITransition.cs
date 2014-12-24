using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.StateMachineTool.Core
{
    public interface ITransition : IGraphElement
    {
        INode Source { get; }
        INode Target { get; }
    }
}
