using System.Text;

namespace Bitcraft.ToolKit.CodeGeneration.CSharp;

public class CSharpVariableDeclarationCodeGenerator : VariableDeclarationCodeGenerator
{
    public CSharpVariableDeclarationCodeGenerator(ILanguageAbstraction languageAbstraction, AccessModifier accessModifier, string[] additionalModifiers, string type, string name, string initializationStatement)
        : base(languageAbstraction, accessModifier, additionalModifiers, type, name, initializationStatement)
    {
    }

    public CSharpVariableDeclarationCodeGenerator(ILanguageAbstraction languageAbstraction, AccessModifier accessModifier, string[] additionalModifiers, string type, string name, ICodeGenerator innerGenerator)
        : base(languageAbstraction, accessModifier, additionalModifiers, type, name, innerGenerator)
    {
    }

    public override void Write(CodeWriter writer)
    {
        var elements = new List<string>();

        string? accessModifierStr = CSharpCodeGenerationUtility.AccessModifierToString(accessModifier);
        if (accessModifierStr != null)
            elements.Add(accessModifierStr);

        if (additionalModifiers != null)
            elements.AddRange(additionalModifiers);

        elements.Add(type);
        elements.Add(name);

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

        writer.AppendLine(string.Join(" ", elements) + ";");
    }
}
