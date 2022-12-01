namespace Bitcraft.ToolKit.CodeGeneration.Cpp;

public class CppUsingCodeGenerator : UsingCodeGenerator
{
    private static readonly string[] separators = new string[] { ".", "::" };

    public CppUsingCodeGenerator(ILanguageAbstraction languageAbstraction, params string[] usings)
        : base(languageAbstraction, usings)
    {
    }

    public override void Write(CodeWriter writer)
    {
        foreach (var u in usings.Where(u => u != null))
        {
            string[] parts = u.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            writer.AppendLine($"using namespace {string.Join("::", parts)};");
        }
    }
}
