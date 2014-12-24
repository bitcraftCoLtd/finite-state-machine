using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration
{
    public struct ArgumentInfo
    {
        public readonly string Type;
        public readonly string Name;

        public ArgumentInfo(string type, string name)
        {
            CodeGenerationUtility.CheckNullOrWhitespaceArgument(type, "type");
            CodeGenerationUtility.CheckValidIdentifierArgument(name, "name");

            Type = type.Trim();
            Name = name;
        }
    }

    public abstract class MethodDeclarationCodeGenerator : ICodeGenerator
    {
        public ILanguageAbstraction Language { get; private set; }
        protected readonly AccessModifier accessModifier;
        protected readonly bool isStatic;
        protected readonly string[] additionalModifiers;
        protected readonly string returnType;
        protected readonly string name;
        protected readonly ArgumentInfo[] arguments;
        protected readonly ScopeCodeGenerator bodyGenerator;

        public MethodDeclarationCodeGenerator(ILanguageAbstraction languageAbstraction, AccessModifier accessModifier, bool isStatic, string[] additionalModifiers, string returnType, string name, ArgumentInfo[] arguments, ScopeCodeGenerator bodyGenerator)
        {
            if (languageAbstraction == null)
                throw new ArgumentNullException("languageAbstraction");
            CodeGenerationUtility.CheckValidIdentifierArgument(name, "name");

            Language = languageAbstraction;

            this.accessModifier = accessModifier;
            this.isStatic = isStatic;
            this.additionalModifiers = additionalModifiers;
            if (string.IsNullOrWhiteSpace(returnType) == false)
                this.returnType = returnType.Trim();
            this.name = name;
            this.arguments = arguments;
            this.bodyGenerator = bodyGenerator;
        }

        public abstract void Write(CodeWriter writer);
    }
}
