namespace Bitcraft.ToolKit.CodeGeneration.CSharp;

public class CSharpCommentCodeGenerator : CommentCodeGenerator
{
    public CSharpCommentCodeGenerator(ILanguageAbstraction languageAbstraction, ICodeGenerator innerGenerator, bool isSingleLine)
        : base(languageAbstraction, innerGenerator, isSingleLine)
    {
    }

    public override void Write(CodeWriter writer)
    {
        if (isSingleLine)
        {
            writer.Append("// ");
            using (writer.SuspendIndentation())
                innerGenerator.Write(writer);
        }
        else
        {
            writer.AppendLine("/*");
            innerGenerator.Write(writer);
            writer.AppendLine("*/");
        }
    }
}
