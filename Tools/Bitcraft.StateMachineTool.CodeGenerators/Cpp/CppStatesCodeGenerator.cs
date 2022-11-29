using System;
using Bitcraft.ToolKit.CodeGeneration;
using Bitcraft.StateMachineTool.Core;
using Bitcraft.ToolKit.CodeGeneration.Cpp;

namespace Bitcraft.StateMachineTool.CodeGenerators.Cpp
{
    public class CppStatesCodeGenerator : CodeGeneratorBase
    {
        private readonly string stateMachineName;
        private readonly IGraph graph;

        public CppStatesCodeGenerator(ILanguageAbstraction languageAbstraction, string stateMachineName, IGraph graph)
            : base(languageAbstraction, null, null)
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            this.stateMachineName = stateMachineName;
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
            foreach (INode node in graph.Nodes)
            {
                string relativeFilename = $"{stateMachineName}{node.Semantic}{Constants.StateSuffix}.autogen.h";

                new CppRawStatementCodeGenerator(Language, $"#include \"./{relativeFilename}\"").Write(writer);
            }
        }
    }
}
