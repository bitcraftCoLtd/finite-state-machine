namespace Bitcraft.ToolKit.CodeGeneration.Cpp;

public class CppRawStatementCodeGenerator : RawStatementCodeGenerator
{
    public CppRawStatementCodeGenerator(ILanguageAbstraction languageAbstraction, string rawStatement)
        : base(languageAbstraction, rawStatement)
    {
    }

    public override void Write(CodeWriter writer)
    {
        writer.AppendLine(rawStatement);
    }
}
