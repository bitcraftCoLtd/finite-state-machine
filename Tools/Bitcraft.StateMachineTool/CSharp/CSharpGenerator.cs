using Bitcraft.StateMachineTool.CodeGenerators;
using Bitcraft.StateMachineTool.CodeGenerators.CSharp;
using Bitcraft.StateMachineTool.Core;
using Bitcraft.ToolKit.CodeGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.StateMachineTool.CSharp
{
    public class CSharpGenerator : IGenerator
    {
        public void Generate(GeneratorOptions options)
        {
            var fs = new FileDefinitions(options.StateMachineName, options.Graph);

            foreach (var folder in fs.Folders)
                Directory.CreateDirectory(Path.Combine(options.OutputPath, folder));

            ILanguageAbstraction generatorsFactory = new ToolKit.CodeGeneration.CSharp.CSharpLanguageAbstraction();

            Utils.WriteFile(new CSharpStateMachineCodeGenerator(generatorsFactory, options.NamespaceName, options.StateMachineName, options.UseOriginalStateBase, options.IsInternal, options.InitialNode, options.Graph), options.OutputPath, fs.StateMachineFilename);
            Utils.WriteFile(new CSharpStateTokensCodeGenerator(generatorsFactory, options.NamespaceName, options.StateMachineName, options.IsInternal, options.Graph), options.OutputPath, fs.StateTokensFilename);
            Utils.WriteFile(new CSharpActionTokensCodeGenerator(generatorsFactory, options.NamespaceName, options.StateMachineName, options.IsInternal, options.Graph), options.OutputPath, fs.ActionTokensFilename);

            foreach (var state in fs.States)
            {
                Utils.WriteFile(
                    new CSharpStateCodeGenerator(
                        generatorsFactory,
                        options.NamespaceName != null ? $"{options.NamespaceName}.{Constants.StatesFolder}" : null,
                        options.StateMachineName,
                        state.Semantic,
                        options.UseOriginalStateBase,
                        options.IsInternal,
                        options.Graph
                    ),
                    options.OutputPath,
                    state.RelativePath
                );
            }
        }
    }
}
