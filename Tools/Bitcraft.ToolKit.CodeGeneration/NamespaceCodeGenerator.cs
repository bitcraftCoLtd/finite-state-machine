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

        public NamespaceCodeGenerator(ILanguageAbstraction languageAbstraction, string namespaceName)
        {
            if (languageAbstraction == null)
                throw new ArgumentNullException(nameof(languageAbstraction));
            CodeGenerationUtility.CheckNullOrWhitespaceArgument(namespaceName, nameof(namespaceName));

            Language = languageAbstraction;

            this.namespaceName = namespaceName;
        }

        public abstract void Write(CodeWriter writer);
    }
}
