using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitcraft.StateMachineTool.Core;

namespace Bitcraft.StateMachineTool.CodeGenerators.CSharp
{
    public class StateInfo
    {
        public string Semantic;
        public string RelativePath;
    }

    public class FileDefinitions
    {
        public string[] Folders { get; private set; }

        public string StateMachineFilename { get; private set; }
        public string StateTokensFilename { get; private set; }
        public string ActionTokensFilename { get; private set; }

        public StateInfo[] States { get; private set; }

        public FileDefinitions(string stateMachineName, IGraph graph)
        {
            Folders = new string[]
            {
                Constants.StatesFolder
            };

            StateMachineFilename = stateMachineName + Constants.StateMachineSuffix + ".autogen.cs";
            StateTokensFilename = stateMachineName + Constants.StateTokensClass + ".autogen.cs";
            ActionTokensFilename = stateMachineName + Constants.ActionTokensClass + ".autogen.cs";

            States = graph.Nodes
                .Where(x => x.IsFinal == false)
                .Select(n => new StateInfo
                {
                    Semantic = n.Semantic,
                    RelativePath = Constants.StatesFolder + "\\" + stateMachineName + n.Semantic + Constants.StateSuffix + ".autogen.cs"
                })
                .ToArray();
        }
    }
}
