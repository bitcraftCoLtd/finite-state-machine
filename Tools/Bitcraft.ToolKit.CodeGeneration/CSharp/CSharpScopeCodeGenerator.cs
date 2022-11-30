namespace Bitcraft.ToolKit.CodeGeneration.CSharp;

public class CSharpScopeCodeGenerator : ScopeCodeGenerator
{
    public CSharpScopeCodeGenerator(ILanguageAbstraction languageAbstraction, ScopeContentType scopeContentType, ICodeGenerator? innerGenerator, bool closeWithNewLine)
        : base(languageAbstraction, scopeContentType, innerGenerator, closeWithNewLine)
    {
    }

    public override void Write(CodeWriter writer)
    {
        writer.AppendLine("{");

        if (innerGenerator != null)
        {
            using (writer.Indent())
                innerGenerator.Write(writer);
        }

        if (closeWithNewLine)
            writer.AppendLine("}");
        else
            writer.Append("}");
    }
}
