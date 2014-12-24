using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration
{
    public abstract class NamespaceCodeGenerator : ICodeGenerator
    {
        public ILanguageAbstraction Language { get; private set; }
        protected readonly string namespaceName;

        public NamespaceCodeGenerator(ILanguageAbstraction languageAbstraction, string namespaceName)
        {
            if (languageAbstraction == null)
                throw new ArgumentNullException("languageAbstraction");
            CodeGenerationUtility.CheckNullOrWhitespaceArgument(namespaceName, "namespaceName");

            Language = languageAbstraction;

            this.namespaceName = namespaceName;
        }

        public abstract void Write(CodeWriter writer);
    }
}
