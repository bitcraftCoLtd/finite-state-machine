using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitcraft.StateMachineTool.Core;
using Bitcraft.ToolKit.CodeGeneration;

namespace Bitcraft.StateMachineTool.CodeGenerators.CSharp
{
    public class StateTokensCodeGenerator : CodeGeneratorBase
    {
        private IGraph graph;

        public StateTokensCodeGenerator(ILanguageAbstraction generatorsFactory, string namespaceName, string stateMachineName, IGraph graph)
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
                stateMachineName + Constants.StateTokensClass,
                null).Write(writer);

            Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, WriteClassContent), true).Write(writer);
        }

        private void WriteClassContent(CodeWriter writer)
        {
            var nodes = graph.Nodes
                .Where(x => x.IsFinal == false)
                .ToArray();

            if (nodes.Length == 0)
                return;

            foreach (var node in nodes)
            {
                Language.CreateVariableDeclarationCodeGenerator(
                    AccessModifier.Public,
                    new[] { "static", "readonly" },
                    Constants.StateTokenType,
                    node.Semantic,
                    string.Format("new " + Constants.StateTokenType + "(\"{0}\")", node.Semantic)).Write(writer);
            }

            writer.AppendLine();

            Language.CreateVariableDeclarationCodeGenerator(
                AccessModifier.Public,
                new[] { "static", "readonly" },
                Constants.StateTokenType + "[]",
                Constants.TokenItemsProperty,
                new AnonymousCodeGenerator(Language, w =>
                {
                    w.AppendLine();
                    Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, w2 =>
                    {
                        foreach (var node in nodes.Take(nodes.Length - 1))
                            w2.AppendLine(node.Semantic + ",");
                        w2.AppendLine(nodes.Last().Semantic);
                    }), false).Write(w);
                })).Write(writer);
        }
    }
}
