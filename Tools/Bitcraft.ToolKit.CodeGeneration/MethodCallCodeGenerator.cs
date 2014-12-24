using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration
{
    public abstract class MethodCallCodeGenerator : ICodeGenerator
    {
        public ILanguageAbstraction Language { get; private set; }
        protected readonly string name;
        protected readonly string[] parameters;

        public MethodCallCodeGenerator(ILanguageAbstraction languageAbstraction, string name, params string[] parameters)
        {
            if (languageAbstraction == null)
                throw new ArgumentNullException("languageAbstraction");

            CodeGenerationUtility.CheckValidIdentifierArgument(name, "name");

            Language = languageAbstraction;

            this.name = name;
            this.parameters = parameters;
        }

        public abstract void Write(CodeWriter writer);
    }
}
