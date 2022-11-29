using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitcraft.ToolKit.CodeGeneration;
using Bitcraft.StateMachineTool.Core;
using Bitcraft.ToolKit.CodeGeneration.Cpp;

namespace Bitcraft.StateMachineTool.CodeGenerators.Cpp
{
    public class CppStateCodeGenerator : CodeGeneratorBase
    {
        private readonly CppFileType cppFileType;
        private readonly string projectRelativePathPrefix;
        private readonly string generatedCodeRelativePathPrefix;
        private readonly string stateName;
        private readonly IGraph graph;
        private readonly bool useStateBase;

        private const string StateDataType = "StateData";

        public CppStateCodeGenerator(ILanguageAbstraction languageAbstraction, CppFileType cppFileType, string projectRelativePathPrefix, string generatedCodeRelativePathPrefix, string namespaceName, string stateMachineName, string stateName, bool useStateBase, IGraph graph)
            : base(languageAbstraction, namespaceName, stateMachineName)
        {
            CodeGenerationUtility.CheckValidPartialIdentifierArgument(stateName, nameof(stateName));

            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            this.cppFileType = cppFileType;
            this.projectRelativePathPrefix = projectRelativePathPrefix;
            this.generatedCodeRelativePathPrefix = generatedCodeRelativePathPrefix;
            this.stateName = stateName;
            this.graph = graph;
            this.useStateBase = useStateBase;
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
                writer.AppendLine($"#include \"{generatedCodeRelativePathPrefix}/{Constants.StatesFolder}/{stateMachineName}{stateName}{Constants.StateSuffix}.autogen.h\"");
                writer.AppendLine($"#include \"{generatedCodeRelativePathPrefix}/{stateMachineName}{Constants.StateTokensClass}.autogen.h\"");
                writer.AppendLine($"#include \"{generatedCodeRelativePathPrefix}/{stateMachineName}{Constants.ActionTokensClass}.autogen.h\"");
                writer.AppendLine($"#include \"StateMachine/state_machine.h\"");
            }
            else if (cppFileType == CppFileType.Header)
            {
                writer.AppendLine($"#include \"{projectRelativePathPrefix}/{stateMachineName}{Constants.StateBaseType}.h\"");
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

            var baseClassName = Constants.StateBaseType;
            if (useStateBase == false)
                baseClassName = stateMachineName + baseClassName;

            ScopeCodeGenerator classBodyGenerator = Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, WriteClassContent), ScopeContentType.Class, false);

            Language.CreateClassCodeGenerator(
                AccessModifier.None,
                null,
                stateMachineName + stateName + Constants.StateSuffix,
                new[] { baseClassName },
                classBodyGenerator
            ).Write(writer);

            if (cppFileType == CppFileType.Header)
                writer.AppendLine();
        }

        private void WriteClassContent(CodeWriter writer)
        {
            string baseClassName = Constants.StateBaseType;
            if (useStateBase == false)
                baseClassName = stateMachineName + baseClassName;

            string className = stateMachineName + stateName + Constants.StateSuffix;

            Language.CreateConstructorDeclarationCodeGenerator(
                AccessModifier.Public,
                false,
                className,
                className,
                null,
                new ParentConstructorInfo
                {
                    BaseName = baseClassName,
                    Type = ParentConstructorType.Base,
                },
                new[] { $"{stateMachineName}{Constants.StateTokensClass}::{stateName}" },
                Language.CreateScopeCodeGenerator(null, ScopeContentType.Method, true)
            ).Write(writer);

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
                    null,
                    "void",
                    className,
                    Constants.OnInitializedMethod,
                    null,
                    Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, w => WriteOnInitializeMethod(transitions, w)), ScopeContentType.Method, true)
                ).Write(writer);

                writer.AppendLine();

                Language.CreateMethodDeclarationCodeGenerator(
                    AccessModifier.Protected,
                    false,
                    null,
                    "void",
                    className,
                    Constants.PreInitializedMethod,
                    null,
                    Language.CreateScopeCodeGenerator(null, ScopeContentType.Method, true)
                ).Write(writer);

                if (cppFileType == CppFileType.Source)
                    writer.AppendLine();

                Language.CreateMethodDeclarationCodeGenerator(
                    AccessModifier.Protected,
                    false,
                    null,
                    "void",
                    className,
                    Constants.PostInitializedMethod,
                    null,
                    Language.CreateScopeCodeGenerator(null, ScopeContentType.Method, true)
                ).Write(writer);

                writer.AppendLine();

                Language.CreateCommentCodeGenerator(new AnonymousCodeGenerator(Language, w2 =>
                    {
                        foreach (var tr in transitions.Take(transitions.Length - 1))
                        {
                            if (WriteTransitionHandlerMethodCall(tr, w2) == false)
                                continue;

                            if (cppFileType == CppFileType.Source)
                                writer.AppendLine();
                        }
                        WriteTransitionHandlerMethodCall(transitions.Last(), w2);
                    }), false
                ).Write(writer);
            }
        }

        private void WriteOnInitializeMethod(ITransition[] transitions, CodeWriter writer)
        {
            Language.CreateMethodCallCodeGenerator(Constants.PreInitializedMethod).Write(writer);

            writer.AppendLine();

            string stateName = Constants.StateBaseType;
            if (useStateBase == false)
                stateName = $"{stateMachineName}{stateName}";

            writer.AppendLine($"{stateName}::{Constants.OnInitializedMethod}();");

            writer.AppendLine();

            if (transitions != null)
            {
                foreach (var tr in transitions)
                {
                    Language.CreateMethodCallCodeGenerator(Constants.RegisterActionHandlerMethod, new[]
                    {
                        $"{stateMachineName}{Constants.ActionTokensClass}::{tr.Semantic}",
                        $"[this]({StateDataType}* data, {Constants.TransitionInfoType}* transition) {{ this->On{stateMachineName}{tr.Semantic}{Constants.ActionSuffix}(data, transition); }}"
                    }).Write(writer);
                }
            }

            writer.AppendLine();

            Language.CreateMethodCallCodeGenerator(Constants.PostInitializedMethod).Write(writer);
        }

        private bool WriteTransitionHandlerMethodCall(ITransition tr, CodeWriter writer)
        {
            var funcName = $"On{stateMachineName}{tr.Semantic}{Constants.ActionSuffix}";
            var target = graph.Nodes.FirstOrDefault(n => n == tr.Target);

            if (target == null)
                return false;

            string className = stateMachineName + stateName + Constants.StateSuffix;

            Language.CreateMethodDeclarationCodeGenerator(
                AccessModifier.Private,
                false,
                null,
                "void",
                className,
                funcName,
                new[]
                {
                    new ArgumentInfo($"{StateDataType}*", "data"),
                    new ArgumentInfo($"{Constants.TransitionInfoType}*", "transition")
                },
                Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, w =>
                {
                    w.AppendLine($"transition->TargetStateToken = {ConstructStateTokenFullname(target)};");
                }), ScopeContentType.Method, true)
            ).Write(writer);

            return true;
        }

        private string ConstructStateTokenFullname(INode node)
        {
            if (node.IsFinal)
                return "null";
            return $"{stateMachineName}{Constants.StateTokensClass}::{node.Semantic}";
        }
    }
}
