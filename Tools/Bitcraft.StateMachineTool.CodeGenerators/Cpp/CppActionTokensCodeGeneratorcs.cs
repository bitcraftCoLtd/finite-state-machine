﻿using System;
using System.Linq;
using Bitcraft.ToolKit.CodeGeneration;
using Bitcraft.StateMachineTool.Core;

namespace Bitcraft.StateMachineTool.CodeGenerators.Cpp
{
    public class CppActionTokensCodeGenerator : CodeGeneratorBase
    {
        private readonly IGraph graph;

        public CppActionTokensCodeGenerator(ILanguageAbstraction generatorsFactory, string namespaceName, string stateMachineName, IGraph graph)
            : base(generatorsFactory, namespaceName, stateMachineName)
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            this.graph = graph;
        }

        public override void Write(CodeWriter writer)
        {
            WriteFileHeader(writer);

            Language.CreateUsingCodeGenerator(
                CppConstants.StateMachineNamespace
            ).Write(writer);

            writer.AppendLine();

            base.Write(writer);
        }

        protected override void WriteContent(CodeWriter writer)
        {
            ScopeCodeGenerator classBodyGenerator = Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, WriteClassContent), ScopeContentType.Class, true);

            Language.CreateClassCodeGenerator(
                AccessModifier.None,
                new[] { "static" },
                stateMachineName + Constants.ActionTokensClass,
                null,
                classBodyGenerator
            ).Write(writer);
        }

        private void WriteClassContent(CodeWriter writer)
        {
            if (graph.Transitions.Length == 0)
                return;

            var distinctTransitions = graph.Transitions
                .Select(t => t.Semantic)
                .Distinct()
                .ToArray();

            foreach (var transition in distinctTransitions)
            {
                Language.CreateVariableDeclarationCodeGenerator(
                    AccessModifier.Public,
                    new[] { "static", "const" },
                    $"{Constants.ActionTokenType}*",
                    transition,
                    $"new {Constants.ActionTokenType}(\"{transition}\")"
                ).Write(writer);
            }

            writer.AppendLine();

            Language.CreateVariableDeclarationCodeGenerator(
                AccessModifier.Public,
                new[] { "static", "const" },
                $"{Constants.ActionTokenType}*",
                $"{Constants.TokenItemsProperty}[]",
                new AnonymousCodeGenerator(Language, w =>
                {
                    w.AppendLine();
                    Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, w2 =>
                    {
                        foreach (var transition in distinctTransitions.Take(distinctTransitions.Length - 1))
                            w2.AppendLine(transition + ",");
                        w2.AppendLine(distinctTransitions.Last());
                    }), ScopeContentType.Method, false).Write(w);
                })).Write(writer);
        }
    }
}