namespace Bitcraft.ToolKit.CodeGeneration.CSharp;

public class CSharpRawStatementCodeGenerator : RawStatementCodeGenerator
{
    public CSharpRawStatementCodeGenerator(ILanguageAbstraction languageAbstraction, string rawStatement)
        : base(languageAbstraction, rawStatement)
    {
    }

    public override void Write(CodeWriter writer)
    {
        writer.AppendLine(rawStatement);
    }
}
