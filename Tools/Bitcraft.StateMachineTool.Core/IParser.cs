using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.StateMachineTool.Core
{
    public interface IParser
    {
        IGraph Parse(Stream stream);
    }
}
