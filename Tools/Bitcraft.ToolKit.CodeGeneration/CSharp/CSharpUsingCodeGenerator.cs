namespace Bitcraft.ToolKit.CodeGeneration.CSharp;

public class CSharpUsingCodeGenerator : UsingCodeGenerator
{
    public CSharpUsingCodeGenerator(ILanguageAbstraction languageAbstraction, params string[] usings)
        : base(languageAbstraction, usings)
    {
    }

    public override void Write(CodeWriter writer)
    {
        foreach (var u in usings.Where(u => u != null))
            writer.AppendLine(string.Format("using {0};", u));
    }
}
