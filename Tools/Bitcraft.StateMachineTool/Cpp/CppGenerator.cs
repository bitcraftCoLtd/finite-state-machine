using System;
using Bitcraft.StateMachineTool.CodeGenerators;
using Bitcraft.StateMachineTool.CodeGenerators.Cpp;
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

            var sourceStateMachineCodeGenerator = new CppStateMachineCodeGenerator(sourceLanguageAbstraction, options.NamespaceName, options.StateMachineName, options.UseOriginalStateBase, options.IsInternal, options.InitialNode, options.Graph);
            var headerStateMachineCodeGenerator = new CppStateMachineCodeGenerator(headerLanguageAbstraction, options.NamespaceName, options.StateMachineName, options.UseOriginalStateBase, options.IsInternal, options.InitialNode, options.Graph);

            string sourceStateMachineFilename = $"{options.StateMachineName}{Constants.StateMachineSuffix}.autogen.cpp";
            string headerStateMachineFilename = $"{options.StateMachineName}{Constants.StateMachineSuffix}.autogen.h";

            Utils.WriteFile(sourceStateMachineCodeGenerator, options.OutputPath, sourceStateMachineFilename);
            Utils.WriteFile(headerStateMachineCodeGenerator, options.OutputPath, headerStateMachineFilename);
        }
    }
}
