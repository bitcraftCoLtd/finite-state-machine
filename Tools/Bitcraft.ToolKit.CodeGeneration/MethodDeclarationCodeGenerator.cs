namespace Bitcraft.ToolKit.CodeGeneration;

public readonly struct ArgumentInfo
{
    public readonly string Type;
    public readonly string Name;

    public ArgumentInfo(string type, string name)
    {
        CodeGenerationUtility.CheckNullOrWhitespaceArgument(type, nameof(type));
        CodeGenerationUtility.CheckValidIdentifierArgument(name, nameof(name));

        Type = type.Trim();
        Name = name;
    }
}

public abstract class MethodDeclarationCodeGenerator : ICodeGenerator
{
    public ILanguageAbstraction Language { get; }

    protected readonly AccessModifier accessModifier;
    protected readonly bool isStatic;
    protected readonly string[]? additionalModifiers;
    protected readonly string? returnType;
    protected readonly string? className;
    protected readonly string name;
    protected readonly ArgumentInfo[]? arguments;
    protected readonly ScopeCodeGenerator? bodyGenerator;

    public MethodDeclarationCodeGenerator(ILanguageAbstraction languageAbstraction, AccessModifier accessModifier, bool isStatic, string[]? additionalModifiers, string? returnType, string? className, string name, ArgumentInfo[]? arguments, ScopeCodeGenerator? bodyGenerator)
    {
        if (languageAbstraction == null)
            throw new ArgumentNullException(nameof(languageAbstraction));
        CodeGenerationUtility.CheckValidIdentifierArgument(name, nameof(name));

        Language = languageAbstraction;

        this.accessModifier = accessModifier;
        this.isStatic = isStatic;
        this.additionalModifiers = additionalModifiers;
        if (string.IsNullOrWhiteSpace(returnType) == false)
            this.returnType = returnType.Trim();
        this.className = className;
        this.name = name;
        this.arguments = arguments;
        this.bodyGenerator = bodyGenerator;
    }

    public abstract void Write(CodeWriter writer);
}
