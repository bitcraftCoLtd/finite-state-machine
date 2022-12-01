using System.Text;

namespace Bitcraft.ToolKit.CodeGeneration.Cpp;

public class CppVariableDeclarationCodeGenerator : VariableDeclarationCodeGenerator
{
    private readonly CppFileType cppFileType;

    public CppVariableDeclarationCodeGenerator(ILanguageAbstraction languageAbstraction, CppFileType cppFileType, AccessModifier accessModifier, string[] additionalModifiers, string type, string name, string initializationStatement)
        : base(languageAbstraction, accessModifier, additionalModifiers, type, name, initializationStatement)
    {
        this.cppFileType = cppFileType;
    }

    public CppVariableDeclarationCodeGenerator(ILanguageAbstraction languageAbstraction, CppFileType cppFileType, AccessModifier accessModifier, string[] additionalModifiers, string type, string name, ICodeGenerator innerGenerator)
        : base(languageAbstraction, accessModifier, additionalModifiers, type, name, innerGenerator)
    {
        this.cppFileType = cppFileType;
    }

    public override void Write(CodeWriter writer)
    {
        if (cppFileType == CppFileType.Source)
            WriteSource(writer);
        else if (cppFileType == CppFileType.Header)
            WriteHeader(writer);
    }

    private void WriteSource(CodeWriter writer)
    {
        var elements = new List<string>
        {
            name
        };

        if (initializationStatement != null)
        {
            elements.Add("=");
            elements.Add(initializationStatement);
        }
        else if (innerGenerator != null)
        {
            var innerSb = new StringBuilder();
            innerGenerator.Write(writer.Clone(innerSb));
            elements.Add("=");
            elements.Add(innerSb.ToString());
        }

        writer.AppendLine($"{string.Join(" ", elements)};");
    }

    private void WriteHeader(CodeWriter writer)
    {
        var elements = new List<string>();

        string? accessModifierStr = CppCodeGenerationUtility.AccessModifierToString(accessModifier);

        if (accessModifierStr != null)
            elements.Add($"{accessModifierStr}:");

        elements.Add(type);
        elements.Add(name);

        writer.AppendLine($"{string.Join(" ", elements)};");
    }
}
