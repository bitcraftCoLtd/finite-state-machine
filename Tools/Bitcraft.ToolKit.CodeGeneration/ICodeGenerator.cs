using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration
{
    public interface ICodeGenerator
    {
        void Write(CodeWriter writer);
    }
}
