using System;

namespace Bitcraft.ToolKit.CodeGeneration.Cpp
{
    public class CppRawStatementCodeGenerator : RawStatementCodeGenerator
    {
        private readonly CppFileType cppFileType;

        public CppRawStatementCodeGenerator(ILanguageAbstraction languageAbstraction, CppFileType cppFileType, string rawStatement)
            : base(languageAbstraction, rawStatement)
        {
            this.cppFileType = cppFileType;
        }

        public override void Write(CodeWriter writer)
        {
            writer.AppendLine(rawStatement + ";");
        }
    }
}
