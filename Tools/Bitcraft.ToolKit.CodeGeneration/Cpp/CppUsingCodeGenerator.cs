using System;
using System.Linq;

namespace Bitcraft.ToolKit.CodeGeneration.Cpp
{
    public class CppUsingCodeGenerator : UsingCodeGenerator
    {
        private readonly CppFileType cppFileType;

        private static readonly string[] separators = new string[] { ".", "::" };

        public CppUsingCodeGenerator(ILanguageAbstraction languageAbstraction, CppFileType cppFileType, params string[] usings)
            : base(languageAbstraction, usings)
        {
            this.cppFileType = cppFileType;
        }

        public override void Write(CodeWriter writer)
        {
            if (cppFileType == CppFileType.Source)
            {
                foreach (var u in usings.Where(u => u != null))
                {
                    string[] parts = u.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                    writer.AppendLine($"using namespace {string.Join("::", parts)};");
                }
            }
        }
    }
}
