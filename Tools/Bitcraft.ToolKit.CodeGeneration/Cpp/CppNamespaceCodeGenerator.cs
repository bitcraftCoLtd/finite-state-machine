using System;

namespace Bitcraft.ToolKit.CodeGeneration.Cpp
{
    public class CppNamespaceCodeGenerator : NamespaceCodeGenerator
    {
        private static readonly string[] separators = new string[] { ".", "::" };

        public CppNamespaceCodeGenerator(ILanguageAbstraction languageAbstraction, CppFileType cppFileType, string namespaceName)
            : base(languageAbstraction, namespaceName)
        {
            _ = cppFileType;
        }

        public override void Write(CodeWriter writer)
        {
            string[] parts = namespaceName.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            string namespaceStatement = string.Join(" { namespace ", parts);

            writer.AppendLine($"namespace {namespaceStatement}");
        }
    }
}
