﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitcraft.ToolKit.CodeGeneration;
using Bitcraft.StateMachineTool.Core;

namespace Bitcraft.StateMachineTool.CodeGenerators.CSharp
{
    public class StateCodeGenerator : CodeGeneratorBase
    {
        private string stateName;
        private IGraph graph;
        private bool useStateBase;
        private bool isInternal;

        public StateCodeGenerator(ILanguageAbstraction generatorsFactory, string namespaceName, string stateMachineName, string stateName, bool useStateBase, bool isInternal, IGraph graph)
            : base(generatorsFactory, namespaceName, stateMachineName)
        {
            CodeGenerationUtility.CheckValidPartialIdentifierArgument(stateName, nameof(stateName));

            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            this.stateName = stateName;
            this.graph = graph;
            this.useStateBase = useStateBase;
            this.isInternal = isInternal;
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
            var baseClassName = Constants.StateBaseType;
            if (useStateBase == false)
                baseClassName = stateMachineName + baseClassName;

            Language.CreateClassCodeGenerator(
                AccessModifier.None,
                new[] { "partial" },
                stateMachineName + stateName + Constants.StateSuffix,
                new[] { baseClassName }).Write(writer);

            Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, WriteClassContent), true).Write(writer);
        }

        private void WriteClassContent(CodeWriter writer)
        {
            Language.CreateConstructorDeclarationCodeGenerator(
                isInternal ? AccessModifier.Internal : AccessModifier.Public,
                false,
                stateMachineName + stateName + Constants.StateSuffix,
                null,
                ParentConstructorType.Base,
                new[] { stateMachineName + Constants.StateTokensClass + "." + stateName },
                Language.CreateScopeCodeGenerator(null, true)).Write(writer);

            var node = graph.Nodes.FirstOrDefault(n => n.Semantic == stateName);

            ITransition[] transitions = null;
            if (node != null)
                transitions = graph.Transitions.Where(tr => tr.Source == node).ToArray();

            if (transitions != null && transitions.Length > 0)
            {
                writer.AppendLine();

                Language.CreateMethodDeclarationCodeGenerator(
                    AccessModifier.Protected,
                    false,
                    new[] { "override" },
                    "void",
                    Constants.OnInitializedMethod,
                    null,
                    Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, w => WriteOnInitializeMethod(transitions, w)), true)
                ).Write(writer);

                writer.AppendLine();

                Language.CreateMethodDeclarationCodeGenerator(
                    AccessModifier.None,
                    false,
                    new[] { "partial" },
                    "void",
                    Constants.PreInitializedMethod,
                    null,
                    null).Write(writer);

                Language.CreateMethodDeclarationCodeGenerator(
                    AccessModifier.None,
                    false,
                    new[] { "partial" },
                    "void",
                    Constants.PostInitializedMethod,
                    null,
                    null).Write(writer);

                writer.AppendLine();

                Language.CreateCommentCodeGenerator(new AnonymousCodeGenerator(Language, w2 =>
                    {
                        foreach (var tr in transitions.Take(transitions.Length - 1))
                        {
                            if (WriteTransitionHandlerMethodCall(tr, w2) == false)
                                continue;
                            writer.AppendLine();
                        }
                        WriteTransitionHandlerMethodCall(transitions.Last(), w2);
                    }), false).Write(writer);
            }
        }

        private void WriteOnInitializeMethod(ITransition[] transitions, CodeWriter writer)
        {
            Language.CreateMethodCallCodeGenerator(Constants.PreInitializedMethod).Write(writer);

            writer.AppendLine();

            Language.CreateRawStatementCodeGenerator("base." + Constants.OnInitializedMethod + "()").Write(writer);

            if (transitions != null)
            {
                foreach (var tr in transitions)
                {
                    Language.CreateMethodCallCodeGenerator(Constants.RegisterActionHandlerMethod, new[]
                    {
                        stateMachineName + Constants.ActionTokensClass + "." + tr.Semantic,
                        "On" + stateMachineName + tr.Semantic + Constants.ActionSuffix
                    }).Write(writer);
                }
            }

            writer.AppendLine();

            Language.CreateMethodCallCodeGenerator(Constants.PostInitializedMethod).Write(writer);
        }

        private bool WriteTransitionHandlerMethodCall(ITransition tr, CodeWriter writer)
        {
            var funcName = "On" + stateMachineName + tr.Semantic + Constants.ActionSuffix;
            var target = graph.Nodes.FirstOrDefault(n => n == tr.Target);

            if (target == null)
                return false;

            Language.CreateMethodDeclarationCodeGenerator(
                AccessModifier.Private,
                false,
                null,
                "void",
                funcName,
                new[]
                {
                    new ArgumentInfo(Constants.StateDataType, "data"),
                    new ArgumentInfo("Action<" + Constants.StateTokenType + ">", "callback")
                },
                Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, w =>
                {
                    Language.CreateMethodCallCodeGenerator(
                        "callback",
                        ConstructStateTokenFullname(target)).Write(w);
                }), true)
            ).Write(writer);

            return true;
        }

        private string ConstructStateTokenFullname(INode node)
        {
            if (node.IsFinal)
                return "null";
            return stateMachineName + Constants.StateTokensClass + "." + node.Semantic;
        }
    }
}
