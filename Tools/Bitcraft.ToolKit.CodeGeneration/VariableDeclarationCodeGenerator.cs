using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration
{
    public abstract class VariableDeclarationCodeGenerator : ICodeGenerator
    {
        public ILanguageAbstraction Language { get; private set; }

        protected AccessModifier accessModifier { get; private set; }
        protected string[] additionalModifiers { get; private set; }
        protected string type { get; private set; }
        protected string name { get; private set; }
        protected string initializationStatement { get; private set; }
        protected ICodeGenerator innerGenerator { get; private set; }

        public VariableDeclarationCodeGenerator(ILanguageAbstraction languageAbstraction, AccessModifier accessModifier, string[] additionalModifiers, string type, string name, string initializationStatement)
        {
            Initialization(languageAbstraction, accessModifier, additionalModifiers, type, name);

            if (string.IsNullOrWhiteSpace(initializationStatement) == false)
                this.initializationStatement = initializationStatement.Trim();
        }

        public VariableDeclarationCodeGenerator(ILanguageAbstraction languageAbstraction, AccessModifier accessModifier, string[] additionalModifiers, string type, string name, ICodeGenerator innerGenerator)
        {
            Initialization(languageAbstraction, accessModifier, additionalModifiers, type, name);

            this.innerGenerator = innerGenerator;
        }

        private void Initialization(ILanguageAbstraction languageAbstraction, AccessModifier accessModifier, string[] additionalModifiers, string type, string name)
        {
            if (languageAbstraction == null)
                throw new ArgumentNullException("languageAbstraction");

            CodeGenerationUtility.CheckNullOrWhitespaceArgument(type, "type");
            CodeGenerationUtility.CheckValidIdentifierArgument(name, "name");

            Language = languageAbstraction;

            this.accessModifier = accessModifier;
            this.additionalModifiers = additionalModifiers;
            this.type = type.Trim();
            this.name = name;
        }

        public abstract void Write(CodeWriter writer);
    }
}
