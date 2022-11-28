using System;
using System.Collections.Generic;
using System.Linq;

namespace Bitcraft.ToolKit.CodeGeneration.Cpp
{
    public class CppMethodDeclarationCodeGenerator : MethodDeclarationCodeGenerator
    {
        private readonly CppFileType cppFileType;

        public CppMethodDeclarationCodeGenerator(ILanguageAbstraction languageAbstraction, CppFileType cppFileType, AccessModifier accessModifier, bool isStatic, string[] additionalModifiers, string returnType, string className, string name, ArgumentInfo[] arguments, ScopeCodeGenerator bodyGenerator)
            : base(languageAbstraction, accessModifier, isStatic, additionalModifiers, returnType, className, name, arguments, bodyGenerator)
        {
            this.cppFileType = cppFileType;
        }

        protected virtual void ConstructArguments(List<string> outArguments)
        {
            if (arguments != null)
            {
                foreach (var a in arguments.Where(p => p.Name != null))
                    outArguments.Add($"{a.Type} {a.Name}");
            }
        }

        private void WriteSource(CodeWriter writer)
        {
            if (isStatic)
                throw new NotSupportedException("C++ does not support static constructors.");

            string returnTypeStr = returnType != null ? $"{returnType} " : string.Empty;

            var arguments = new List<string>();
            ConstructArguments(arguments);

            string argumentsStr = string.Join(", ", arguments);

            writer.Append($"{returnTypeStr}{className}::{name}({argumentsStr})");

            WriteBody(writer);
        }

        private void WriteHeader(CodeWriter writer)
        {
            if (isStatic)
                throw new NotSupportedException("C++ does not support static constructors.");

            if (returnType == null)
                throw new ArgumentException("Method declaration must have a return type.");

            string accessModifierStr = CppCodeGenerationUtility.AccessModifierToString(accessModifier);
            accessModifierStr = accessModifierStr != null ? $"{accessModifierStr}: " : string.Empty;

            var arguments = new List<string>();
            ConstructArguments(arguments);

            string argumentsStr = string.Join(", ", arguments);

            var proto = $"{accessModifierStr}{returnType} {name}({argumentsStr});";

            writer.AppendLine(proto);
        }

        protected virtual void WriteBody(CodeWriter writer)
        {
            if (bodyGenerator != null)
            {
                using (writer.SuspendIndentation())
                    writer.AppendLine();
                bodyGenerator.Write(writer);
            }
            else
            {
                using (writer.SuspendIndentation())
                    writer.AppendLine(";");
            }
        }

        public override void Write(CodeWriter writer)
        {
            if (cppFileType == CppFileType.Source)
                WriteSource(writer);
            else if (cppFileType == CppFileType.Header)
                WriteHeader(writer);
        }
    }
}
