namespace Bitcraft.ToolKit.CodeGeneration.Cpp;

public class CppNamespaceCodeGenerator : NamespaceCodeGenerator
{
    private static readonly string[] separators = new string[] { ".", "::" };

    public CppNamespaceCodeGenerator(ILanguageAbstraction languageAbstraction, string namespaceName, bool closeWithNewLine, ScopeCodeGenerator bodyGenerator)
        : base(languageAbstraction, namespaceName, closeWithNewLine, bodyGenerator)
    {
    }

    public override void Write(CodeWriter writer)
    {
        string[] parts = namespaceName.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        string namespaceStatement = string.Join(" { namespace ", parts);

        writer.AppendLine($"namespace {namespaceStatement}");

        if (bodyGenerator != null)
            bodyGenerator.Write(writer);

        using (writer.SuspendIndentation())
        {
            for (int i = 0; i < parts.Length - 1; i++)
                writer.Append(" }");
        }

        if (closeWithNewLine)
            writer.AppendLine();
    }
}
