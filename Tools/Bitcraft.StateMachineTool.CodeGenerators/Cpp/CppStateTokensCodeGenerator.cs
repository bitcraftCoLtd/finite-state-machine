using Bitcraft.StateMachineTool.Core;
using Bitcraft.ToolKit.CodeGeneration;
using Bitcraft.ToolKit.CodeGeneration.Cpp;

namespace Bitcraft.StateMachineTool.CodeGenerators.Cpp;

public class CppStateTokensCodeGenerator : CodeGeneratorBase
{
    private readonly CppFileType cppFileType;
    private readonly string generatedCodeRelativePathPrefix;
    private readonly string? stateMachineRelativePathPrefix;
    private readonly IGraph graph;

    public CppStateTokensCodeGenerator(ILanguageAbstraction languageAbstraction, CppFileType cppFileType, string generatedCodeRelativePathPrefix, string? stateMachineRelativePathPrefix, string? namespaceName, string stateMachineName, IGraph graph)
        : base(languageAbstraction, namespaceName, stateMachineName)
    {
        if (graph == null)
            throw new ArgumentNullException(nameof(graph));

        this.cppFileType = cppFileType;
        this.generatedCodeRelativePathPrefix = generatedCodeRelativePathPrefix;
        this.stateMachineRelativePathPrefix = stateMachineRelativePathPrefix;
        this.graph = graph;
    }

    public override void Write(CodeWriter writer)
    {
        if (cppFileType == CppFileType.Header)
        {
            Language.CreateRawStatementCodeGenerator("#pragma once").Write(writer);
            writer.AppendLine();
        }

        WriteFileHeader(writer);

        string prefix = string.Empty;

        if (stateMachineRelativePathPrefix != null)
            prefix = $"{stateMachineRelativePathPrefix}/";

        if (cppFileType == CppFileType.Source)
            writer.AppendLine($"#include \"{generatedCodeRelativePathPrefix}/{stateMachineName}{Constants.StateTokensClass}.autogen.h\"");

        writer.AppendLine($"#include \"{prefix}ax-fsm/state_token.h\"");

        writer.AppendLine();

        base.Write(writer);
    }

    protected override void WriteContent(CodeWriter writer)
    {
        Language.CreateUsingCodeGenerator(
            CppConstants.StateMachineNamespace
        ).Write(writer);

        writer.AppendLine();

        ScopeCodeGenerator classBodyGenerator = Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, WriteClassContent), ScopeContentType.Class, false);

        Language.CreateClassCodeGenerator(
            AccessModifier.Public,
            null,
            stateMachineName + Constants.StateTokensClass,
            null,
            classBodyGenerator
        ).Write(writer);

        if (cppFileType == CppFileType.Header)
            writer.AppendLine();
    }

    private void WriteClassContent(CodeWriter writer)
    {
        var nodes = graph.Nodes
            .Where(x => x.IsFinal == false)
            .ToArray();

        if (nodes.Length == 0)
            return;

        if (cppFileType == CppFileType.Source)
        {
            foreach (var node in nodes)
                writer.AppendLine($"{Constants.StateTokenType}* {stateMachineName}{Constants.StateTokensClass}::{node.Semantic} = new {Constants.StateTokenType}(L\"{node.Semantic}\");");
        }
        else if (cppFileType == CppFileType.Header)
        {
            foreach (var node in nodes)
                writer.AppendLine($"public: static {Constants.StateTokenType}* {node.Semantic};");
        }

        writer.AppendLine();

        if (cppFileType == CppFileType.Source)
        {
            writer.AppendLine($"{Constants.StateTokenType}* {stateMachineName}{Constants.StateTokensClass}::Items[] =");

            Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, w =>
            {
                foreach (var node in nodes.Take(nodes.Length - 1))
                    w.AppendLine($"{stateMachineName}{Constants.StateTokensClass}::{node.Semantic},");
                w.AppendLine($"{stateMachineName}{Constants.StateTokensClass}::{nodes.Last().Semantic}");
            }), ScopeContentType.Method, false).Write(writer);

            using (writer.SuspendIndentation())
                writer.AppendLine(";");
        }
        else if (cppFileType == CppFileType.Header)
        {
            writer.AppendLine($"public: static {Constants.StateTokenType}* Items[];");
        }
    }
}
