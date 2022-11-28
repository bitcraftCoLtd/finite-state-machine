using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration
{
    public abstract class NamespaceCodeGenerator : ICodeGenerator
    {
        public ILanguageAbstraction Language { get; }
        protected readonly string namespaceName;
        protected readonly bool closeWithNewLine;
        protected readonly ScopeCodeGenerator bodyGenerator;

        public NamespaceCodeGenerator(ILanguageAbstraction languageAbstraction, string namespaceName, bool closeWithNewLine, ScopeCodeGenerator bodyGenerator)
        {
            if (languageAbstraction == null)
                throw new ArgumentNullException(nameof(languageAbstraction));
            CodeGenerationUtility.CheckNullOrWhitespaceArgument(namespaceName, nameof(namespaceName));

            Language = languageAbstraction;

            this.namespaceName = namespaceName;
            this.closeWithNewLine = closeWithNewLine;
            this.bodyGenerator = bodyGenerator;
        }

        public abstract void Write(CodeWriter writer);
    }
}
