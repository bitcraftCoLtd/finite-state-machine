using Bitcraft.StateMachineTool.Core;
using Bitcraft.ToolKit.CodeGeneration;
using System;
using System.Linq;

namespace Bitcraft.StateMachineTool.CodeGenerators.CSharp
{
    public class CSharpStateMachineCodeGenerator : CodeGeneratorBase
    {
        private readonly IGraph graph;
        private readonly INode initialNode;
        private readonly bool useStateBase;
        private readonly bool isInternal;

        public CSharpStateMachineCodeGenerator(ILanguageAbstraction generatorsFactory, string namespaceName, string stateMachineName, bool useStateBase, bool isInternal, INode initialNode, IGraph graph)
            : base(generatorsFactory, namespaceName, stateMachineName)
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            this.graph = graph;
            this.initialNode = initialNode;
            this.useStateBase = useStateBase;
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
            ScopeCodeGenerator classBodyGenerator = Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, WriteClassContent), ScopeContentType.Class, true);

            Language.CreateClassCodeGenerator(
                AccessModifier.None,
                new[] { "partial" },
                stateMachineName + Constants.StateMachineSuffix,
                new[] { Constants.StateManagerType },
                classBodyGenerator
            ).Write(writer);
        }

        private void WriteClassContent(CodeWriter writer)
        {
            string baseClassName = Constants.StateBaseType;
            if (useStateBase == false)
                baseClassName = stateMachineName + baseClassName;

            string className = stateMachineName + Constants.StateMachineSuffix;

            Language.CreateConstructorDeclarationCodeGenerator(
                isInternal ? AccessModifier.Internal : AccessModifier.Public,
                false,
                className,
                className,
                null,
                new ParentConstructorInfo
                {
                    BaseName = baseClassName,
                    Type = ParentConstructorType.This,
                },
                new[] { "null" },
                Language.CreateScopeCodeGenerator(null, ScopeContentType.Method, true)).Write(writer);

            writer.AppendLine();

            Language.CreateConstructorDeclarationCodeGenerator(
                isInternal ? AccessModifier.Internal : AccessModifier.Public,
                false,
                className,
                className,
                new[] { new ArgumentInfo("object", "context") },
                new ParentConstructorInfo
                {
                    BaseName = baseClassName,
                    Type = ParentConstructorType.Base,
                },
                new[] { "context" },
                Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, WriteConstructorContent), ScopeContentType.Method, true)).Write(writer);

            writer.AppendLine();

            Language.CreateMethodDeclarationCodeGenerator(
                AccessModifier.None,
                false,
                new[] { "partial" },
                "void",
                className,
                Constants.PreHandlersRegistrationMethod,
                null,
                null).Write(writer);

            Language.CreateMethodDeclarationCodeGenerator(
                AccessModifier.None,
                false,
                new[] { "partial" },
                "void",
                className,
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

            if (initialNode != null)
            {
                writer.AppendLine();

                Language.CreateMethodCallCodeGenerator(
                    Constants.SetInitialStateMethod,
                    stateMachineName + Constants.StateTokensClass + "." + initialNode.Semantic
                ).Write(writer);
            }
        }
    }
}
