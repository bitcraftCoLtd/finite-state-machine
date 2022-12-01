namespace Bitcraft.ToolKit.CodeGeneration.CSharp;

public class CSharpClassCodeGenerator : ClassCodeGenerator
{
    public CSharpClassCodeGenerator(
        ILanguageAbstraction languageAbstraction,
        AccessModifier accessModifier,
        string[]? additionalModifiers,
        string name,
        string[]? bases,
        ScopeCodeGenerator bodyGenerator
    )
        : base(languageAbstraction, accessModifier, additionalModifiers, name, bases, bodyGenerator)
    {
    }

    public override void Write(CodeWriter writer)
    {
        var elements = new List<string>();

        var accessModifierStr = CSharpCodeGenerationUtility.AccessModifierToString(accessModifier);
        if (accessModifierStr != null)
            elements.Add(accessModifierStr);

        if (additionalModifiers != null)
        {
            foreach (var m in additionalModifiers.Where(x => string.IsNullOrWhiteSpace(x) == false))
                elements.Add(m.Trim());
        }

        elements.Add("class");

        elements.Add(name);

        if (bases != null)
        {
            var newBases = bases
                .Where(x => string.IsNullOrWhiteSpace(x) == false)
                .Select(x => x.Trim())
                .ToArray();

            if (newBases.Length > 0)
            {
                elements.Add(":");
                foreach (var b in newBases)
                    elements.Add(b);
            }
        }

        writer.AppendLine(string.Join(" ", elements));

        bodyGenerator?.Write(writer);
    }
}
