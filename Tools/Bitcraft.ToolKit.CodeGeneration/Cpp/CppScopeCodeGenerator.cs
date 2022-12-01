namespace Bitcraft.ToolKit.CodeGeneration.Cpp;

public class CppScopeCodeGenerator : ScopeCodeGenerator
{
    private readonly CppFileType cppFileType;

    public CppScopeCodeGenerator(ILanguageAbstraction languageAbstraction, CppFileType cppFileType, ScopeContentType scopeContentType, ICodeGenerator? innerGenerator, bool closeWithNewLine)
        : base(languageAbstraction, scopeContentType, innerGenerator, closeWithNewLine)
    {
        this.cppFileType = cppFileType;
    }

    private void WriteWithoutScope(CodeWriter writer)
    {
        if (innerGenerator != null)
            innerGenerator.Write(writer);
    }

    private void WriteWithScope(CodeWriter writer)
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

    public override void Write(CodeWriter writer)
    {
        if (cppFileType == CppFileType.Source && scopeContentType == ScopeContentType.Class)
            WriteWithoutScope(writer);
        else
            WriteWithScope(writer);
    }
}
