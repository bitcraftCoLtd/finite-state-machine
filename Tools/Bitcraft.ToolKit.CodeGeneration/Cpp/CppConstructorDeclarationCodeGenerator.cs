namespace Bitcraft.ToolKit.CodeGeneration.Cpp;

public class CppConstructorDeclarationCodeGenerator : ConstructorDeclarationCodeGenerator
{
    private readonly CppFileType cppFileType;

    public CppConstructorDeclarationCodeGenerator(
        ILanguageAbstraction languageAbstraction,
        CppFileType cppFileType,
        AccessModifier accessModifier,
        bool isStatic,
        string? className,
        string name,
        ArgumentInfo[]? arguments,
        ParentConstructorInfo? parentConstructorInfo,
        string[]? parentConstructorParameters,
        ScopeCodeGenerator bodyGenerator)
        : base(languageAbstraction, accessModifier, isStatic, className, name, arguments, parentConstructorInfo, parentConstructorParameters, bodyGenerator)
    {
        this.cppFileType = cppFileType;
    }

    protected virtual void ConstructModifiers(List<string> outModifiers)
    {
        if (isStatic)
            throw new NotSupportedException("C++ does not support static constructors.");

        string? accessModifierStr = CppCodeGenerationUtility.AccessModifierToString(accessModifier);

        if (accessModifierStr != null)
            outModifiers.Add($"{accessModifierStr}: ");
    }

    protected virtual void ConstructArguments(List<string> outArguments)
    {
        if (arguments != null)
        {
            foreach (ArgumentInfo a in arguments.Where(p => p.Name != null))
                outArguments.Add($"{a.Type} {a.Name}");
        }
    }

    public override void Write(CodeWriter writer)
    {
        if (cppFileType == CppFileType.Source)
            WriteSource(writer);
        else if (cppFileType == CppFileType.Header)
            WriteHeader(writer);
    }

    private void WriteSource(CodeWriter writer)
    {
        writer.Append($"{className}::{name}(");

        using (writer.SuspendIndentation())
        {
            var arguments = new List<string>();
            ConstructArguments(arguments);

            if (arguments.Count > 0)
                writer.Append(string.Join(", ", arguments));

            writer.Append(")");
        }

        if (parentConstructorInfo != null && parentConstructorInfo.Value.Type != ParentConstructorType.None)
        {
            writer.AppendLine();

            using (writer.Indent())
            {
                writer.Append($": {parentConstructorInfo.Value.BaseName}(");

                using (writer.SuspendIndentation())
                {
                    if (parentConstructorParameters != null)
                    {
                        writer.Append(string.Join(", ", parentConstructorParameters
                            .Where(x => string.IsNullOrWhiteSpace(x) == false)
                            .Select(x => x.Trim())
                        ));
                    }

                    writer.Append(")");
                }
            }
        }

        WriteBody(writer);
    }

    private void WriteHeader(CodeWriter writer)
    {
        string? accessModifierStr = CppCodeGenerationUtility.AccessModifierToString(accessModifier);

        if (accessModifierStr != null)
            writer.Append($"{accessModifierStr}:");
        else
            writer.Append(string.Empty);

        using (writer.SuspendIndentation())
        {
            if (additionalModifiers != null)
            {
                foreach (string additionalModifier in additionalModifiers)
                    writer.Append($" {additionalModifier}");
            }

            writer.Append($" {name}(");

            var arguments = new List<string>();
            ConstructArguments(arguments);

            if (arguments.Count > 0)
                writer.Append(string.Join(", ", arguments));

            writer.AppendLine(");");
        }
    }

    protected virtual void WriteBody(CodeWriter writer)
    {
        if (bodyGenerator != null)
        {
            writer.AppendLine();
            bodyGenerator.Write(writer);
        }
        else
        {
            using (writer.SuspendIndentation())
                writer.AppendLine(";");
        }
    }
}
