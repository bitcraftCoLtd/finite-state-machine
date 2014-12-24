using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration.CSharp
{
    public class CSharpNamespaceCodeGenerator : NamespaceCodeGenerator
    {
        public CSharpNamespaceCodeGenerator(ILanguageAbstraction languageAbstraction, string namespaceName)
            : base(languageAbstraction, namespaceName)
        {
        }

        public override void Write(CodeWriter writer)
        {
            writer.AppendLine("namespace {0}", namespaceName);
        }
    }
}
