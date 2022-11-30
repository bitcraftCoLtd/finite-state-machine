using Bitcraft.StateMachineTool.CodeGenerators;
using Bitcraft.StateMachineTool.Core;

namespace Bitcraft.StateMachineTool.CSharp;

public class StateInfo
{
    public required string Semantic { get; init; }
    public required string RelativePath { get; init; }
}

public class FileDefinitions
{
    public string[] Folders { get; }

    public string StateMachineFilename { get; }
    public string StateTokensFilename { get; }
    public string ActionTokensFilename { get; }

    public StateInfo[] States { get; }

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
