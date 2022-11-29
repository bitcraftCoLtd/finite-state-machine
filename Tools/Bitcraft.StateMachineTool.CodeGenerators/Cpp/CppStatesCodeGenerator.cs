using System;
using Bitcraft.ToolKit.CodeGeneration;
using Bitcraft.StateMachineTool.Core;
using Bitcraft.ToolKit.CodeGeneration.Cpp;
using System.Linq;

namespace Bitcraft.StateMachineTool.CodeGenerators.Cpp
{
    public class CppStatesCodeGenerator : CodeGeneratorBase
    {
        private readonly IGraph graph;

        public CppStatesCodeGenerator(ILanguageAbstraction languageAbstraction, string stateMachineName, IGraph graph)
            : base(languageAbstraction, null, stateMachineName)
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            this.graph = graph;
        }

        public override void Write(CodeWriter writer)
        {
            Language.CreateRawStatementCodeGenerator("#pragma once").Write(writer);
            writer.AppendLine();

            WriteFileHeader(writer);

            base.Write(writer);
        }

        protected override void WriteContent(CodeWriter writer)
        {
            var orderedNodeNames = graph.Nodes
                .Select(node => $"{stateMachineName}{node.Semantic}{Constants.StateSuffix}.autogen.h")
                .OrderBy(x => x);

            foreach (string filename in orderedNodeNames)
            {
                new CppRawStatementCodeGenerator(Language, $"#include \"./{filename}\"").Write(writer);
            }
        }
    }
}
