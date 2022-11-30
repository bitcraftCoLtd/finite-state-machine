namespace Bitcraft.ToolKit.CodeGeneration;

public abstract class CommentCodeGenerator : ICodeGenerator
{
    public ILanguageAbstraction Language { get; }

    protected readonly ICodeGenerator innerGenerator;
    protected readonly bool isSingleLine;

    public CommentCodeGenerator(ILanguageAbstraction languageAbstraction, ICodeGenerator innerGenerator, bool isSingleLine)
    {
        if (languageAbstraction == null)
            throw new ArgumentNullException(nameof(languageAbstraction));
        if (innerGenerator == null)
            throw new ArgumentNullException(nameof(innerGenerator));

        Language = languageAbstraction;

        this.innerGenerator = innerGenerator;
        this.isSingleLine = isSingleLine;
    }

    public abstract void Write(CodeWriter writer);
}
