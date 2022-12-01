namespace Bitcraft.ToolKit.CodeGeneration;

public readonly struct ParentConstructorInfo
{
    public required ParentConstructorType Type { get; init; }
    public required string BaseName { get; init; }
}

public enum ParentConstructorType
{
    None,
    Base,
    This
}

public abstract class ConstructorDeclarationCodeGenerator : MethodDeclarationCodeGenerator
{
    protected readonly ParentConstructorInfo? parentConstructorInfo;
    protected readonly string[] parentConstructorParameters;

    protected ConstructorDeclarationCodeGenerator(
        ILanguageAbstraction languageAbstraction,
        AccessModifier accessModifier,
        bool isStatic,
        string? className,
        string name,
        ArgumentInfo[]? arguments,
        ParentConstructorInfo? parentConstructorInfo,
        string[]? parentConstructorParameters,
        ScopeCodeGenerator bodyGenerator)
        : base(languageAbstraction, accessModifier, isStatic, null, null, className, name, arguments, bodyGenerator)
    {
        this.parentConstructorInfo = parentConstructorInfo;
        this.parentConstructorParameters = parentConstructorParameters ?? Array.Empty<string>();
    }

    public abstract override void Write(CodeWriter writer);
}
