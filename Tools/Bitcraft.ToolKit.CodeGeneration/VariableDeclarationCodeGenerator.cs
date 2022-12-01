namespace Bitcraft.ToolKit.CodeGeneration;

public abstract class VariableDeclarationCodeGenerator : ICodeGenerator
{
    public ILanguageAbstraction Language { get; }

    protected readonly AccessModifier accessModifier;
    protected readonly string[] additionalModifiers;
    protected readonly string type;
    protected readonly string name;
    protected readonly string? initializationStatement = null;
    protected readonly ICodeGenerator? innerGenerator = null;

    private VariableDeclarationCodeGenerator(ILanguageAbstraction languageAbstraction, AccessModifier accessModifier, string[] additionalModifiers, string type, string name)
    {
        if (languageAbstraction == null)
            throw new ArgumentNullException(nameof(languageAbstraction));

        CodeGenerationUtility.CheckNullOrWhitespaceArgument(type, nameof(type));
        CodeGenerationUtility.CheckValidIdentifierArgument(name, nameof(name));

        Language = languageAbstraction;

        this.accessModifier = accessModifier;
        this.additionalModifiers = additionalModifiers;
        this.type = type.Trim();
        this.name = name;
    }

    public VariableDeclarationCodeGenerator(ILanguageAbstraction languageAbstraction, AccessModifier accessModifier, string[] additionalModifiers, string type, string name, string initializationStatement)
        : this(languageAbstraction, accessModifier, additionalModifiers, type, name)
    {
        if (string.IsNullOrWhiteSpace(initializationStatement) == false)
            this.initializationStatement = initializationStatement.Trim();
    }

    public VariableDeclarationCodeGenerator(ILanguageAbstraction languageAbstraction, AccessModifier accessModifier, string[] additionalModifiers, string type, string name, ICodeGenerator innerGenerator)
        : this(languageAbstraction, accessModifier, additionalModifiers, type, name)
    {
        this.innerGenerator = innerGenerator;
    }

    public abstract void Write(CodeWriter writer);
}
