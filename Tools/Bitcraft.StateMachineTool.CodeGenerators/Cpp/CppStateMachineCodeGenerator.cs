using Bitcraft.StateMachineTool.Core;
using Bitcraft.ToolKit.CodeGeneration;
using System;
using System.Linq;

namespace Bitcraft.StateMachineTool.CodeGenerators.Cpp
{
    public class CppStateMachineCodeGenerator : CodeGeneratorBase
    {
        private readonly IGraph graph;
        private readonly INode initialNode;
        private readonly bool useStateBase;

        public CppStateMachineCodeGenerator(ILanguageAbstraction generatorsFactory, string namespaceName, string stateMachineName, bool useStateBase, INode initialNode, IGraph graph)
            : base(generatorsFactory, namespaceName, stateMachineName)
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            this.graph = graph;
            this.initialNode = initialNode;
            this.useStateBase = useStateBase;
        }

        public override void Write(CodeWriter writer)
        {
            WriteFileHeader(writer);

            Language.CreateUsingCodeGenerator(
                CppConstants.StateMachineNamespace,
                $"{namespaceName}::{Constants.StatesFolder}"
            ).Write(writer);

            writer.AppendLine();

            base.Write(writer);
        }

        protected override void WriteContent(CodeWriter writer)
        {
            ScopeCodeGenerator classBodyGenerator = Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, WriteClassContent), ScopeContentType.Class, true);

            Language.CreateClassCodeGenerator(
                AccessModifier.None,
                null,
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

            Language.CreateConstructorDeclarationCodeGenerator(
                AccessModifier.Public,
                false,
                $"{stateMachineName}{Constants.StateMachineSuffix}",
                null,
                new ParentConstructorInfo
                {
                    BaseName = baseClassName,
                    Type = ParentConstructorType.This,
                },
                new[] { "nullptr" },
                Language.CreateScopeCodeGenerator(null, ScopeContentType.Method, true)).Write(writer);

            writer.AppendLine();

            Language.CreateConstructorDeclarationCodeGenerator(
                AccessModifier.Public,
                false,
                stateMachineName + Constants.StateMachineSuffix,
                new[] { new ArgumentInfo("void*", "context") },
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
                null,
                "void",
                Constants.PreHandlersRegistrationMethod,
                null,
                null).Write(writer);

            Language.CreateMethodDeclarationCodeGenerator(
                AccessModifier.None,
                false,
                null,
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
                    $"new {stateMachineName}{node.Semantic}{Constants.StateSuffix}()").Write(writer);
            }

            writer.AppendLine();

            Language.CreateMethodCallCodeGenerator(Constants.PostHandlersRegistrationMethod).Write(writer);

            if (initialNode != null)
            {
                writer.AppendLine();

                Language.CreateMethodCallCodeGenerator(
                    Constants.SetInitialStateMethod,
                    $"{stateMachineName}{Constants.StateTokensClass}::{initialNode.Semantic}"
                ).Write(writer);
            }
        }
    }
}
