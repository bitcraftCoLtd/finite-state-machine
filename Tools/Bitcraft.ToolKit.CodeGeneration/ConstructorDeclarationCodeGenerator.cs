using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration
{
    public enum ParentConstructorType
    {
        None,
        Base,
        This
    }

    public abstract class ConstructorDeclarationCodeGenerator : MethodDeclarationCodeGenerator
    {
        protected readonly ParentConstructorType parentConstructorType;
        protected readonly string[] parentConstructorParameters;

        protected ConstructorDeclarationCodeGenerator(
            ILanguageAbstraction languageAbstraction,
            AccessModifier accessModifier,
            bool isStatic,
            string name,
            ArgumentInfo[] arguments,
            ParentConstructorType parentConstructorType,
            string[] parentConstructorParameters,
            ScopeCodeGenerator bodyGenerator)
            : base(languageAbstraction, accessModifier, isStatic, null, null, name, arguments, bodyGenerator)
        {
            this.parentConstructorType = parentConstructorType;
            this.parentConstructorParameters = parentConstructorParameters ?? new string[0];
        }

        public abstract override void Write(CodeWriter writer);
    }
}
