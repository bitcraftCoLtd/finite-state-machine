using System;
using System.IO;
using System.Linq;
using Bitcraft.StateMachineTool.CodeGenerators;
using Bitcraft.StateMachineTool.CodeGenerators.Cpp;
using Bitcraft.StateMachineTool.CodeGenerators.CSharp;
using Bitcraft.ToolKit.CodeGeneration;
using Bitcraft.ToolKit.CodeGeneration.Cpp;

namespace Bitcraft.StateMachineTool.Cpp
{
    public class CppGenerator : IGenerator
    {
        public void Generate(GeneratorOptions options)
        {
            ILanguageAbstraction sourceLanguageAbstraction = new CppLanguageAbstraction(CppFileType.Source);
            ILanguageAbstraction headerLanguageAbstraction = new CppLanguageAbstraction(CppFileType.Header);

            GenerateStateMachineCode(sourceLanguageAbstraction, headerLanguageAbstraction, options);
            GenerateStateTokensCode(sourceLanguageAbstraction, headerLanguageAbstraction, options);
            GenerateActionTokensCode(sourceLanguageAbstraction, headerLanguageAbstraction, options);

            GenerateStatesCode(sourceLanguageAbstraction, headerLanguageAbstraction, options);
        }

        private void GenerateStateMachineCode(ILanguageAbstraction sourceLanguageAbstraction, ILanguageAbstraction headerLanguageAbstraction, GeneratorOptions options)
        {
            var sourceStateMachineCodeGenerator = new CppStateMachineCodeGenerator(sourceLanguageAbstraction, CppFileType.Source, options.NamespaceName, options.StateMachineName, options.UseOriginalStateBase, options.InitialNode, options.Graph);
            var headerStateMachineCodeGenerator = new CppStateMachineCodeGenerator(headerLanguageAbstraction, CppFileType.Header, options.NamespaceName, options.StateMachineName, options.UseOriginalStateBase, options.InitialNode, options.Graph);

            string sourceStateMachineFilename = $"{options.StateMachineName}{Constants.StateMachineSuffix}.autogen.cpp";
            string headerStateMachineFilename = $"{options.StateMachineName}{Constants.StateMachineSuffix}.autogen.h";

            Utils.WriteFile(sourceStateMachineCodeGenerator, options.OutputPath, sourceStateMachineFilename);
            Utils.WriteFile(headerStateMachineCodeGenerator, options.OutputPath, headerStateMachineFilename);
        }

        private void GenerateStateTokensCode(ILanguageAbstraction sourceLanguageAbstraction, ILanguageAbstraction headerLanguageAbstraction, GeneratorOptions options)
        {
            var sourceStateTokensCodeGenerator = new CppStateTokensCodeGenerator(sourceLanguageAbstraction, CppFileType.Source, options.NamespaceName, options.StateMachineName, options.Graph);
            var headerStateTokensCodeGenerator = new CppStateTokensCodeGenerator(headerLanguageAbstraction, CppFileType.Header, options.NamespaceName, options.StateMachineName, options.Graph);

            string sourceStateTokensFilename = $"{options.StateMachineName}{Constants.StateTokensClass}.autogen.cpp";
            string headerStateTokensFilename = $"{options.StateMachineName}{Constants.StateTokensClass}.autogen.h";

            Utils.WriteFile(sourceStateTokensCodeGenerator, options.OutputPath, sourceStateTokensFilename);
            Utils.WriteFile(headerStateTokensCodeGenerator, options.OutputPath, headerStateTokensFilename);
        }

        private void GenerateActionTokensCode(ILanguageAbstraction sourceLanguageAbstraction, ILanguageAbstraction headerLanguageAbstraction, GeneratorOptions options)
        {
            var sourceActionTokensCodeGenerator = new CppActionTokensCodeGenerator(sourceLanguageAbstraction, CppFileType.Source, options.NamespaceName, options.StateMachineName, options.Graph);
            var headerActionTokensCodeGenerator = new CppActionTokensCodeGenerator(headerLanguageAbstraction, CppFileType.Header, options.NamespaceName, options.StateMachineName, options.Graph);

            string sourceActionTokensFilename = $"{options.StateMachineName}{Constants.ActionTokensClass}.autogen.cpp";
            string headerActionTokensFilename = $"{options.StateMachineName}{Constants.ActionTokensClass}.autogen.h";

            Utils.WriteFile(sourceActionTokensCodeGenerator, options.OutputPath, sourceActionTokensFilename);
            Utils.WriteFile(headerActionTokensCodeGenerator, options.OutputPath, headerActionTokensFilename);
        }

        private void GenerateStatesCode(ILanguageAbstraction sourceLanguageAbstraction, ILanguageAbstraction headerLanguageAbstraction, GeneratorOptions options)
        {
            string allStatesHeaderRelativePath = Path.Combine(Constants.StatesFolder, $"{options.StateMachineName}{Constants.StateSuffix}s.autogen.h");
            Utils.WriteFile(new CppStatesCodeGenerator(headerLanguageAbstraction, options.StateMachineName, options.Graph), options.OutputPath, allStatesHeaderRelativePath);

            var states = options.Graph.Nodes
                .Where(x => x.IsFinal == false)
                .Select(n => new
                {
                    Semantic = n.Semantic,
                    SourceRelativePath = Path.Combine(Constants.StatesFolder, $"{options.StateMachineName}{n.Semantic}{Constants.StateSuffix}.autogen.cpp"),
                    HeaderRelativePath = Path.Combine(Constants.StatesFolder, $"{options.StateMachineName}{n.Semantic}{Constants.StateSuffix}.autogen.h"),
                });

            foreach (var state in states)
            {
                var sourceStateCodeGenerator = new CppStateCodeGenerator(
                    sourceLanguageAbstraction,
                    CppFileType.Source,
                    options.NamespaceName != null ? $"{options.NamespaceName}.{Constants.StatesFolder}" : null,
                    options.StateMachineName,
                    state.Semantic,
                    options.UseOriginalStateBase,
                    options.Graph
                );

                var headerStateCodeGenerator = new CppStateCodeGenerator(
                    headerLanguageAbstraction,
                    CppFileType.Header,
                    options.NamespaceName != null ? $"{options.NamespaceName}.{Constants.StatesFolder}" : null,
                    options.StateMachineName,
                    state.Semantic,
                    options.UseOriginalStateBase,
                    options.Graph
                );

                Utils.WriteFile(sourceStateCodeGenerator, options.OutputPath, state.SourceRelativePath);
                Utils.WriteFile(headerStateCodeGenerator, options.OutputPath, state.HeaderRelativePath);
            }
        }
    }
}
