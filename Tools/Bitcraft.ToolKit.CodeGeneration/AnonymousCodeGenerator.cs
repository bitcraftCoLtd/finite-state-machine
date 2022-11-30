namespace Bitcraft.ToolKit.CodeGeneration;

public class AnonymousCodeGenerator : ICodeGenerator
{
    public ILanguageAbstraction Language { get; }

    private readonly Action<CodeWriter> codeGenerator;

    public AnonymousCodeGenerator(ILanguageAbstraction languageAbstraction, Action<CodeWriter> codeGenerator)
    {
        if (languageAbstraction == null)
            throw new ArgumentNullException(nameof(languageAbstraction));
        if (codeGenerator == null)
            throw new ArgumentNullException(nameof(codeGenerator));

        Language = languageAbstraction;

        this.codeGenerator = codeGenerator;
    }

    public void Write(CodeWriter writer)
    {
        codeGenerator(writer);
    }
}
