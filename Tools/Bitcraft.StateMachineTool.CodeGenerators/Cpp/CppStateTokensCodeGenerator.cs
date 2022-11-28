using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitcraft.StateMachineTool.Core;
using Bitcraft.ToolKit.CodeGeneration;

namespace Bitcraft.StateMachineTool.CodeGenerators.Cpp
{
    public class CppStateTokensCodeGenerator : CodeGeneratorBase
    {
        private readonly IGraph graph;

        public CppStateTokensCodeGenerator(ILanguageAbstraction generatorsFactory, string namespaceName, string stateMachineName, IGraph graph)
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
                AccessModifier.Public,
                null,
                stateMachineName + Constants.StateTokensClass,
                null,
                classBodyGenerator
            ).Write(writer);
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
                    new[] { "static", "const" },
                    Constants.StateTokenType,
                    node.Semantic,
                    $"new {Constants.StateTokenType}(\"{node.Semantic}\")"
                ).Write(writer);
            }

            writer.AppendLine();

            Language.CreateVariableDeclarationCodeGenerator(
                AccessModifier.Public,
                new[] { "static", "const" },
                $"{Constants.StateTokenType}[]",
                Constants.TokenItemsProperty,
                new AnonymousCodeGenerator(Language, w =>
                {
                    w.AppendLine();
                    Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, w2 =>
                    {
                        foreach (var node in nodes.Take(nodes.Length - 1))
                            w2.AppendLine(node.Semantic + ",");
                        w2.AppendLine(nodes.Last().Semantic);
                    }), ScopeContentType.Method, false).Write(w);
                })).Write(writer);
        }
    }
}
