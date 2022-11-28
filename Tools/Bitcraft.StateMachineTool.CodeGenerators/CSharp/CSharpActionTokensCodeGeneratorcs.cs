using System;
using System.Linq;
using Bitcraft.ToolKit.CodeGeneration;
using Bitcraft.StateMachineTool.Core;

namespace Bitcraft.StateMachineTool.CodeGenerators.CSharp
{
    public class CSharpActionTokensCodeGenerator : CodeGeneratorBase
    {
        private readonly IGraph graph;
        private readonly bool isInternal;

        public CSharpActionTokensCodeGenerator(ILanguageAbstraction generatorsFactory, string namespaceName, string stateMachineName, bool isInternal, IGraph graph)
            : base(generatorsFactory, namespaceName, stateMachineName)
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            this.isInternal = isInternal;
            this.graph = graph;
        }

        public override void Write(CodeWriter writer)
        {
            WriteFileHeader(writer);

            Language.CreateUsingCodeGenerator(
                "System",
                CSharpConstants.StateMachineNamespace
            ).Write(writer);

            writer.AppendLine();

            base.Write(writer);
        }

        protected override void WriteContent(CodeWriter writer)
        {
            ScopeCodeGenerator classBodyGenerator = Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, WriteClassContent), ScopeContentType.Class, true);

            Language.CreateClassCodeGenerator(
                isInternal ? AccessModifier.Internal : AccessModifier.Public,
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
                    isInternal ? AccessModifier.Internal : AccessModifier.Public,
                    new[] { "static", "readonly" },
                    Constants.ActionTokenType,
                    transition,
                    string.Format("new " + Constants.ActionTokenType + "(\"{0}\")", transition)).Write(writer);
            }

            writer.AppendLine();

            Language.CreateVariableDeclarationCodeGenerator(
                isInternal ? AccessModifier.Internal : AccessModifier.Public,
                new[] { "static", "readonly" },
                Constants.ActionTokenType + "[]",
                Constants.TokenItemsProperty,
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
