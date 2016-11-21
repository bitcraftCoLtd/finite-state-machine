using Bitcraft.StateMachineTool.Core;
using Bitcraft.ToolKit.CodeGeneration;
using System;
using System.Linq;

namespace Bitcraft.StateMachineTool.CodeGenerators.CSharp
{
    public class StateMachineCodeGenerator : CodeGeneratorBase
    {
        private IGraph graph;
        private INode initialNode;
        private bool isInternal;

        public StateMachineCodeGenerator(ILanguageAbstraction generatorsFactory, string namespaceName, string stateMachineName, bool isInternal, INode initialNode, IGraph graph)
            : base(generatorsFactory, namespaceName, stateMachineName)
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            this.graph = graph;
            this.initialNode = initialNode;
            this.isInternal = isInternal;
        }

        public override void Write(CodeWriter writer)
        {
            WriteFileHeader(writer);

            Language.CreateUsingCodeGenerator(
                "System",
                CSharpConstants.StateMachineNamespace,
                namespaceName + "." + Constants.StatesFolder
            ).Write(writer);

            writer.AppendLine();

            base.Write(writer);
        }

        protected override void WriteContent(CodeWriter writer)
        {
            Language.CreateClassCodeGenerator(
                AccessModifier.None,
                new[] { "partial" },
                stateMachineName + Constants.StateMachineSuffix,
                new[] { Constants.StateManagerType }).Write(writer);

            Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, WriteClassContent), true).Write(writer);
        }

        private void WriteClassContent(CodeWriter writer)
        {
            Language.CreateConstructorDeclarationCodeGenerator(
                isInternal ? AccessModifier.Internal : AccessModifier.Public,
                false,
                stateMachineName + Constants.StateMachineSuffix,
                null,
                ParentConstructorType.This,
                new[] { "null" },
                Language.CreateScopeCodeGenerator(null, true)).Write(writer);

            writer.AppendLine();

            Language.CreateConstructorDeclarationCodeGenerator(
                isInternal ? AccessModifier.Internal : AccessModifier.Public,
                false,
                stateMachineName + Constants.StateMachineSuffix,
                new[] { new ArgumentInfo("object", "context") },
                ParentConstructorType.Base,
                new[] { "context" },
                Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, WriteConstructorContent), true)).Write(writer);

            writer.AppendLine();

            Language.CreateMethodDeclarationCodeGenerator(
                AccessModifier.None,
                false,
                new[] { "partial" },
                "void",
                Constants.PreHandlersRegistrationMethod,
                null,
                null).Write(writer);

            Language.CreateMethodDeclarationCodeGenerator(
                AccessModifier.None,
                false,
                new[] { "partial" },
                "void",
                Constants.PostHandlersRegistrationMethod,
                null,
                null).Write(writer);
        }

        private void WriteConstructorContent(CodeWriter writer)
        {
            Language.CreateMethodCallCodeGenerator(Constants.PreHandlersRegistrationMethod).Write(writer);

            writer.AppendLine();

            var nodes = graph.Nodes.Where(x => x.IsFinal == false);

            foreach (var node in nodes)
            {
                Language.CreateMethodCallCodeGenerator(
                    Constants.RegisterStateMethod,
                    "new " + stateMachineName + node.Semantic + Constants.StateSuffix + "()").Write(writer);
            }

            writer.AppendLine();

            Language.CreateMethodCallCodeGenerator(Constants.PostHandlersRegistrationMethod).Write(writer);

            writer.AppendLine();

            if (initialNode != null)
            {
                Language.CreateMethodCallCodeGenerator(
                    Constants.SetInitialStateMethod,
                    stateMachineName + Constants.StateTokensClass + "." + initialNode.Semantic
                ).Write(writer);
            }
        }
    }
}
