using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration
{
    public class ParentConstructorInfo
    {
        public ParentConstructorType Type;
        public string BaseName;
    }

    public enum ParentConstructorType
    {
        None,
        Base,
        This
    }

    public abstract class ConstructorDeclarationCodeGenerator : MethodDeclarationCodeGenerator
    {
        protected readonly ParentConstructorInfo parentConstructorInfo;
        protected readonly string[] parentConstructorParameters;

        protected ConstructorDeclarationCodeGenerator(
            ILanguageAbstraction languageAbstraction,
            AccessModifier accessModifier,
            bool isStatic,
            string className,
            string name,
            ArgumentInfo[] arguments,
            ParentConstructorInfo parentConstructorInfo,
            string[] parentConstructorParameters,
            ScopeCodeGenerator bodyGenerator)
            : base(languageAbstraction, accessModifier, isStatic, null, null, className, name, arguments, bodyGenerator)
        {
            this.parentConstructorInfo = parentConstructorInfo;
            this.parentConstructorParameters = parentConstructorParameters ?? new string[0];
        }

        public abstract override void Write(CodeWriter writer);
    }
}
