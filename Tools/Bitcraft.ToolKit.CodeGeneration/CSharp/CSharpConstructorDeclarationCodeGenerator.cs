﻿namespace Bitcraft.ToolKit.CodeGeneration.CSharp;

public class CSharpConstructorDeclarationCodeGenerator : ConstructorDeclarationCodeGenerator
{
    public CSharpConstructorDeclarationCodeGenerator(
        ILanguageAbstraction languageAbstraction,
        AccessModifier accessModifier,
        bool isStatic,
        string? className,
        string name,
        ArgumentInfo[]? arguments,
        ParentConstructorInfo? parentConstructorInfo,
        string[]? parentConstructorParameters,
        ScopeCodeGenerator bodyGenerator
    )
        : base(languageAbstraction, accessModifier, isStatic, className, name, arguments, parentConstructorInfo, parentConstructorParameters, bodyGenerator)
    {
    }

    protected virtual void ConstructModifiers(List<string> outModifiers)
    {
        if (isStatic)
            outModifiers.Add("static");

        string? accessModifierStr = CSharpCodeGenerationUtility.AccessModifierToString(accessModifier);

        if (accessModifierStr != null)
            outModifiers.Add(accessModifierStr);

        if (additionalModifiers != null)
        {
            foreach (var m in additionalModifiers.Where(x => string.IsNullOrWhiteSpace(x) == false))
                outModifiers.Add(m.Trim());
        }
    }

    protected virtual void ConstructArguments(List<string> outArguments)
    {
        if (arguments != null)
        {
            foreach (var a in arguments.Where(p => p.Name != null))
                outArguments.Add(string.Format("{0} {1}", a.Type, a.Name));
        }
    }

    public override void Write(CodeWriter writer)
    {
        WritePrototype(writer);

        if (parentConstructorInfo != null && parentConstructorInfo.Value.Type != ParentConstructorType.None)
        {
            var parentConstructor = parentConstructorInfo.Value.Type == ParentConstructorType.Base
                ? "base"
                : "this";

            // Prototype is not newline ended.
            writer.AppendLine();

            using (writer.Indent())
            {
                writer.Append(": {0}({1})",
                    parentConstructor,
                    string.Join(", ",
                    (parentConstructorParameters ?? Array.Empty<string>())
                        .Where(x => string.IsNullOrWhiteSpace(x) == false)
                        .Select(x => x.Trim())));
            }
        }

        WriteBody(writer);
    }

    protected virtual void WritePrototype(CodeWriter writer)
    {
        var modifiers = new List<string>();
        ConstructModifiers(modifiers);

        var arguments = new List<string>();
        ConstructArguments(arguments);

        var proto = string.Format("{0}{1} {2}({3})",
            string.Join(" ", modifiers),
            returnType != null ? " " + returnType : "",
            name,
            string.Join(", ", arguments)
        );

        writer.Append(proto);
    }

    protected virtual void WriteBody(CodeWriter writer)
    {
        if (bodyGenerator != null)
        {
            using (writer.SuspendIndentation())
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
