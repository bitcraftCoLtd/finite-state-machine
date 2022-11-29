using System;
using System.Linq;
using Bitcraft.ToolKit.CodeGeneration;
using Bitcraft.StateMachineTool.Core;
using Bitcraft.ToolKit.CodeGeneration.Cpp;

namespace Bitcraft.StateMachineTool.CodeGenerators.Cpp
{
    public class CppActionTokensCodeGenerator : CodeGeneratorBase
    {
        private readonly CppFileType cppFileType;
        private readonly IGraph graph;

        public CppActionTokensCodeGenerator(ILanguageAbstraction languageAbstraction, CppFileType cppFileType, string namespaceName, string stateMachineName, IGraph graph)
            : base(languageAbstraction, namespaceName, stateMachineName)
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            this.cppFileType = cppFileType;
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
                new CppRawStatementCodeGenerator(Language, "#include \"StateMachine/state_machine.h\"").Write(writer);
            }
            else if (cppFileType == CppFileType.Header)
            {
                new CppRawStatementCodeGenerator(Language, "#include \"StateMachine/action_token.h\"").Write(writer);
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
            if (graph.Transitions.Length == 0)
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
                    new CppRawStatementCodeGenerator(Language, statement).Write(writer);
                }
            }
            else if (cppFileType == CppFileType.Header)
            {
                foreach (var transition in distinctTransitions)
                {
                    string statement = $"public: static {Constants.ActionTokenType}* {transition};";
                    new CppRawStatementCodeGenerator(Language, statement).Write(writer);
                }
            }

            writer.AppendLine();

            if (cppFileType == CppFileType.Source)
            {
                string itemsStatement = $"{Constants.ActionTokenType}* {actionTokensClassName}::Items[] =";
                new CppRawStatementCodeGenerator(Language, itemsStatement).Write(writer);

                Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, w =>
                {
                    foreach (var transition in distinctTransitions.Take(distinctTransitions.Length - 1))
                        w.AppendLine($"{actionTokensClassName}::{transition},");
                    w.AppendLine($"{actionTokensClassName}::{distinctTransitions.Last()}");
                }), ScopeContentType.Method, false).Write(writer);
            }
            else if (cppFileType == CppFileType.Header)
            {
                string itemsStatement = $"public: static {Constants.ActionTokenType}* Items[];";
                new CppRawStatementCodeGenerator(Language, itemsStatement).Write(writer);
            }
        }
    }
}
