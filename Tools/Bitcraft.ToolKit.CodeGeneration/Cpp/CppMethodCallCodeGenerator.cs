namespace Bitcraft.ToolKit.CodeGeneration.Cpp;

public class CppMethodCallCodeGenerator : MethodCallCodeGenerator
{
    private readonly CppFileType cppFileType;

    public CppMethodCallCodeGenerator(ILanguageAbstraction languageAbstraction, CppFileType cppFileType, string name, params string[] parameters)
        : base(languageAbstraction, name, parameters)
    {
        this.cppFileType = cppFileType;
    }

    public override void Write(CodeWriter writer)
    {
        if (cppFileType == CppFileType.Source)
        {
            string args = string.Join(", ", parameters.Where(p => string.IsNullOrWhiteSpace(p) == false).Select(p => p.Trim()));

            writer.AppendLine($"{name}({args});");
        }
    }
}
