using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration.CSharp
{
    public class CSharpRawStatementCodeGenerator : RawStatementCodeGenerator
    {
        public CSharpRawStatementCodeGenerator(ILanguageAbstraction languageAbstraction, string rawStatement)
            : base(languageAbstraction, rawStatement)
        {
        }

        public override void Write(CodeWriter writer)
        {
            writer.AppendLine(rawStatement + ";");
        }
    }
}
