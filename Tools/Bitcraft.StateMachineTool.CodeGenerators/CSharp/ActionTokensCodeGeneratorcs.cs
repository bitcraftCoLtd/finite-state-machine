using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitcraft.ToolKit.CodeGeneration;
using Bitcraft.StateMachineTool.Core;

namespace Bitcraft.StateMachineTool.CodeGenerators.CSharp
{
    public class ActionTokensCodeGenerator : CodeGeneratorBase
    {
        private IGraph graph;

        public ActionTokensCodeGenerator(ILanguageAbstraction generatorsFactory, string namespaceName, string stateMachineName, IGraph graph)
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
                "System",
                CSharpConstants.StateMachineNamespace
            ).Write(writer);

            writer.AppendLine();

            base.Write(writer);
        }

        protected override void WriteContent(CodeWriter writer)
        {
            Language.CreateClassCodeGenerator(
                AccessModifier.Public,
                new[] { "static" },
                stateMachineName + Constants.ActionTokensClass,
                null).Write(writer);

            Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, WriteClassContent), true).Write(writer);
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
                    new[] { "static", "readonly" },
                    Constants.ActionTokenType,
                    transition,
                    string.Format("new " + Constants.ActionTokenType + "(\"{0}\")", transition)).Write(writer);
            }

            writer.AppendLine();

            Language.CreateVariableDeclarationCodeGenerator(
                AccessModifier.Public,
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
                            }), false).Write(w);
                    })).Write(writer);
        }
    }
}
