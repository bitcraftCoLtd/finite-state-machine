using Bitcraft.StateMachineTool.Core;
using Bitcraft.ToolKit.CodeGeneration;
using Bitcraft.ToolKit.CodeGeneration.Cpp;
using System;
using System.Linq;

namespace Bitcraft.StateMachineTool.CodeGenerators.Cpp
{
    public class CppStateMachineCodeGenerator : CodeGeneratorBase
    {
        private readonly CppFileType cppFileType;
        private readonly string projectRelativePathPrefix;
        private readonly string generatedCodeRelativePathPrefix;
        private readonly IGraph graph;
        private readonly INode initialNode;
        private readonly bool useStateBase;

        public CppStateMachineCodeGenerator(ILanguageAbstraction languageAbstraction, CppFileType cppFileType, string projectRelativePathPrefix, string generatedCodeRelativePathPrefix, string namespaceName, string stateMachineName, bool useStateBase, INode initialNode, IGraph graph)
            : base(languageAbstraction, namespaceName, stateMachineName)
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            this.cppFileType = cppFileType;
            this.projectRelativePathPrefix = projectRelativePathPrefix;
            this.generatedCodeRelativePathPrefix = generatedCodeRelativePathPrefix;
            this.graph = graph;
            this.initialNode = initialNode;
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
                writer.AppendLine($"#include \"{generatedCodeRelativePathPrefix}/{stateMachineName}{Constants.StateMachineSuffix}.autogen.h\"");
                writer.AppendLine($"#include \"{generatedCodeRelativePathPrefix}/{stateMachineName}{Constants.StateTokensClass}.autogen.h\"");
                writer.AppendLine($"#include \"{generatedCodeRelativePathPrefix}/{Constants.StatesFolder}/{stateMachineName}{Constants.StatesFolder}.autogen.h\"");
                if (useStateBase == false)
                    writer.AppendLine($"#include \"{projectRelativePathPrefix}/{stateMachineName}{Constants.StateBaseType}.h\"");
                writer.AppendLine("#include \"StateMachine/state_machine.h\"");
            }
            else if (cppFileType == CppFileType.Header)
            {
                writer.AppendLine("#include \"StateMachine/state_manager.h\"");
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
                null,
                stateMachineName + Constants.StateMachineSuffix,
                new[] { Constants.StateManagerType },
                classBodyGenerator
            ).Write(writer);

            writer.AppendLine();
        }

        private void WriteClassContent(CodeWriter writer)
        {
            string className = $"{stateMachineName}{Constants.StateMachineSuffix}";

            Language.CreateConstructorDeclarationCodeGenerator(
                AccessModifier.Public,
                false,
                className,
                className,
                null,
                new ParentConstructorInfo
                {
                    BaseName = Constants.StateManagerType,
                    Type = ParentConstructorType.This,
                },
                new[] { "nullptr" },
                Language.CreateScopeCodeGenerator(null, ScopeContentType.Method, true)).Write(writer);

            if (cppFileType == CppFileType.Source)
                writer.AppendLine();

            Language.CreateConstructorDeclarationCodeGenerator(
                AccessModifier.Public,
                false,
                className,
                className,
                new[] { new ArgumentInfo("void*", "context") },
                new ParentConstructorInfo
                {
                    BaseName = Constants.StateManagerType,
                    Type = ParentConstructorType.Base,
                },
                new[] { "context" },
                Language.CreateScopeCodeGenerator(new AnonymousCodeGenerator(Language, WriteConstructorContent), ScopeContentType.Method, true)
            ).Write(writer);

            if (cppFileType == CppFileType.Source)
                writer.AppendLine();

            Language.CreateConstructorDeclarationCodeGenerator(
                AccessModifier.Public,
                false,
                className,
                $"~{className}",
                null,
                null,
                null,
                Language.CreateScopeCodeGenerator(null, ScopeContentType.Method, true)
            ).Write(writer);

            writer.AppendLine();

            Language.CreateMethodDeclarationCodeGenerator(
                AccessModifier.Public,
                false,
                null,
                "void",
                $"{stateMachineName}{Constants.StateMachineSuffix}",
                Constants.PreHandlersRegistrationMethod,
                null,
                Language.CreateScopeCodeGenerator(null, ScopeContentType.Method, true)
            ).Write(writer);

            if (cppFileType == CppFileType.Source)
                writer.AppendLine();

            Language.CreateMethodDeclarationCodeGenerator(
                AccessModifier.Public,
                false,
                null,
                "void",
                className,
                Constants.PostHandlersRegistrationMethod,
                null,
                Language.CreateScopeCodeGenerator(null, ScopeContentType.Method, false)
            ).Write(writer);
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
                    $"new {Constants.StatesFolder}::{stateMachineName}{node.Semantic}{Constants.StateSuffix}()").Write(writer);
            }

            writer.AppendLine();

            Language.CreateMethodCallCodeGenerator(Constants.PostHandlersRegistrationMethod).Write(writer);

            if (initialNode != null)
            {
                writer.AppendLine();

                Language.CreateMethodCallCodeGenerator(
                    Constants.SetInitialStateMethod,
                    $"{stateMachineName}{Constants.StateTokensClass}::{initialNode.Semantic}", "nullptr"
                ).Write(writer);
            }
        }
    }
}
