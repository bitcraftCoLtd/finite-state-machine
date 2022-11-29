using System;

namespace Bitcraft.ToolKit.CodeGeneration.Cpp
{
    public class CppRawStatementCodeGenerator : RawStatementCodeGenerator
    {
        public CppRawStatementCodeGenerator(ILanguageAbstraction languageAbstraction, CppFileType cppFileType, string rawStatement)
            : base(languageAbstraction, rawStatement)
        {
            _ = cppFileType;
        }

        public override void Write(CodeWriter writer)
        {
            writer.AppendLine(rawStatement);
        }
    }
}
