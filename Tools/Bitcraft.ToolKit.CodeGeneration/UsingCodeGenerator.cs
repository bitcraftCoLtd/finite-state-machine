namespace Bitcraft.ToolKit.CodeGeneration;

public abstract class UsingCodeGenerator : ICodeGenerator
{
    public ILanguageAbstraction Language { get; }

    protected readonly string[] usings;

    public UsingCodeGenerator(ILanguageAbstraction languageAbstraction, params string[] usings)
    {
        if (languageAbstraction == null)
            throw new ArgumentNullException(nameof(languageAbstraction));

        Language = languageAbstraction;

        this.usings = usings;
    }

    public abstract void Write(CodeWriter writer);
}
