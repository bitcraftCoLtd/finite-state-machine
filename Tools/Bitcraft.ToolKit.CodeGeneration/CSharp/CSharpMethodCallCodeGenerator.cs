namespace Bitcraft.ToolKit.CodeGeneration.CSharp;

public class CSharpMethodCallCodeGenerator : MethodCallCodeGenerator
{
    public CSharpMethodCallCodeGenerator(ILanguageAbstraction languageAbstraction, string name, params string[] parameters)
        : base(languageAbstraction, name, parameters)
    {
    }

    public override void Write(CodeWriter writer)
    {
        writer.AppendLine("{0}({1});",
            name,
            string.Join(", ", parameters.Where(p => string.IsNullOrWhiteSpace(p) == false).Select(p => p.Trim())));
    }
}
