namespace Bitcraft.ToolKit.CodeGeneration;

public enum AccessModifier
{
    Public,
    Internal,
    Protected,
    ProtectedInternal,
    Private,
    None
}

public abstract class ClassCodeGenerator : ICodeGenerator
{
    public ILanguageAbstraction Language { get; }

    protected readonly AccessModifier accessModifier;
    protected readonly string[]? additionalModifiers;
    protected readonly string name;
    protected readonly string[]? bases;
    protected readonly ScopeCodeGenerator bodyGenerator;

    public ClassCodeGenerator(
        ILanguageAbstraction languageAbstraction,
        AccessModifier accessModifier,
        string[]? additionalModifiers,
        string name,
        string[]? bases,
        ScopeCodeGenerator bodyGenerator
    )
    {
        CodeGenerationUtility.IsValidIdentifier(name);

        Language = languageAbstraction;

        this.accessModifier = accessModifier;
        this.additionalModifiers = additionalModifiers;
        this.name = name;
        this.bases = bases;
        this.bodyGenerator = bodyGenerator;
    }

    public abstract void Write(CodeWriter writer);
}
