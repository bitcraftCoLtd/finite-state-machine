﻿using Bitcraft.ToolKit.CodeGeneration;
using Bitcraft.StateMachineTool.Core;
using Bitcraft.ToolKit.CodeGeneration.Cpp;

namespace Bitcraft.StateMachineTool.CodeGenerators.Cpp;

public class CppActionTokensCodeGenerator : CodeGeneratorBase
{
    private readonly CppFileType cppFileType;
    private readonly string generatedCodeRelativePathPrefix;
    private readonly string? stateMachineRelativePathPrefix;
    private readonly IGraph graph;

    public CppActionTokensCodeGenerator(ILanguageAbstraction languageAbstraction, CppFileType cppFileType, string generatedCodeRelativePathPrefix, string? stateMachineRelativePathPrefix, string? namespaceName, string stateMachineName, IGraph graph)
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

        if (cppFileType == CppFileType.Source)
        {
            writer.AppendLine($"#include \"{generatedCodeRelativePathPrefix}/{stateMachineName}{Constants.ActionTokensClass}.autogen.h\"");
            if (stateMachineRelativePathPrefix != null)
                writer.AppendLine($"#include \"{stateMachineRelativePathPrefix}/StateMachine/state_machine.h\"");
            else
                writer.AppendLine("#include \"StateMachine/state_machine.h\"");
        }
        else if (cppFileType == CppFileType.Header)
        {
            if (stateMachineRelativePathPrefix != null)
                writer.AppendLine($"#include \"{stateMachineRelativePathPrefix}/StateMachine/action_token.h\"");
            else
                writer.AppendLine("#include \"StateMachine/action_token.h\"");
        }

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
            AccessModifier.None,
            new[] { "static" },
            stateMachineName + Constants.ActionTokensClass,
            null,
            classBodyGenerator
        ).Write(writer);

        writer.AppendLine();
    }

    private void WriteClassContent(CodeWriter writer)
    {
        if (graph.Transitions.Count == 0)
            return;

        var distinctTransitions = graph.Transitions
            .Select(t => t.Semantic)
            .Distinct()
            .ToArray();

        string actionTokensClassName = $"{stateMachineName}{Constants.ActionTokensClass}";

        if (cppFileType == CppFileType.Source)
        {
            foreach (var transition in distinctTransitions)
            {
                string statement = $"{Constants.ActionTokenType}* {actionTokensClassName}::{transition} = new {Constants.ActionTokenType}(L\"{transition}\");";
                writer.AppendLine(statement);
            }
        }
        else if (cppFileType == CppFileType.Header)
        {
            foreach (var transition in distinctTransitions)
            {
                string statement = $"public: static {Constants.ActionTokenType}* {transition};";
                writer.AppendLine(statement);
            }
        }

        writer.AppendLine();

        if (cppFileType == CppFileType.Source)
        {
            string itemsStatement = $"{Constants.ActionTokenType}* {actionTokensClassName}::Items[] =";
            writer.AppendLine(itemsStatement);

            Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, w =>
            {
                foreach (var transition in distinctTransitions.Take(distinctTransitions.Length - 1))
                    w.AppendLine($"{actionTokensClassName}::{transition},");
                w.AppendLine($"{actionTokensClassName}::{distinctTransitions.Last()}");
            }), ScopeContentType.Method, false).Write(writer);

            using (writer.SuspendIndentation())
                writer.Append(";");
        }
        else if (cppFileType == CppFileType.Header)
        {
            string itemsStatement = $"public: static {Constants.ActionTokenType}* Items[];";
            writer.AppendLine(itemsStatement);
        }
    }
}
