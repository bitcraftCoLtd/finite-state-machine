using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration.CSharp
{
    public class CSharpScopeCodeGenerator : ScopeCodeGenerator
    {
        public CSharpScopeCodeGenerator(ILanguageAbstraction languageAbstraction, ICodeGenerator innerGenerator, bool closeWithNewLine)
            : base(languageAbstraction, innerGenerator, closeWithNewLine)
        {
        }

        public override void Write(CodeWriter writer)
        {
            writer.AppendLine("{");

            if (innerGenerator != null)
            {
                using (writer.Indent())
                    innerGenerator.Write(writer);
            }

            if (closeWithNewLine)
                writer.AppendLine("}");
            else
                writer.Append("}");
        }
    }
}
