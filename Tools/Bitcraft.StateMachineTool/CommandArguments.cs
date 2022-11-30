using System.Collections.ObjectModel;
using System.Reflection;

namespace Bitcraft.StateMachineTool;

public class CommandArguments
{
    public string? GraphmlFilename { get; private set; }
    public string? NamespaceName { get; private set; }
    public string? StateMachineName { get; private set; }
    public string? OutputFolder { get; private set; }
    public string? InitialStateName { get; private set; }
    public bool IsOutputFolderRelativeToWorkingDir { get; private set; }
    public bool UseOriginalStateBase { get; private set; }
    public bool IsInternal { get; private set; }

    private readonly string[] args;

    public const string HelpArgumentKey = "-help";
    public const string VersionArgumentKey = "-version";
    public const string NamespaceNameArgumentKey = "-ns";
    public const string StateMachineNameArgumentKey = "-name";
    public const string GraphmlFilenameArgumentKey = "-file";
    public const string OutputFolderArgumentKey = "-out";
    public const string InitialStateNameArgumentKey = "-init";
    public const string OutputRelativeToFileArgumentKey = "-fromwd";
    public const string UseOriginalStateBaseArgumentKey = "-statebase";
    public const string InternalArgumentKey = "-internal";
    public const string CustomOptionArgumentKey = "-custom";

    public bool NothingToDo { get; private set; }

    private readonly List<string> errors = new List<string>();
    public IReadOnlyCollection<string> Errors { get; }

    private readonly Dictionary<string, string?> customOptions = new Dictionary<string, string?>();
    public IReadOnlyDictionary<string, string?> CustomOptions { get; }

    public CommandArguments(string[] args)
    {
        if (args == null)
            throw new ArgumentNullException(nameof(args));

        this.args = args;
        Errors = new ReadOnlyCollection<string>(errors);
        CustomOptions = new ReadOnlyDictionary<string, string?>(customOptions);

        Initialize();
    }

    private void Initialize()
    {
        if (args.Length == 0)
            PrintUsage();
        else if (args.Length == 1 && File.Exists(args[0]))
        {
            GraphmlFilename = args[0];
            return;
        }

        if (args.Contains(VersionArgumentKey))
            PrintVersion();

        if (args.Contains(HelpArgumentKey))
            PrintUsage();

        NothingToDo = true;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith("-") == false)
                continue;

            switch (args[i])
            {
                case NamespaceNameArgumentKey:
                    NamespaceName = GetNext(ref i);
                    NothingToDo = false;
                    break;
                case StateMachineNameArgumentKey:
                    StateMachineName = GetNext(ref i);
                    NothingToDo = false;
                    break;
                case GraphmlFilenameArgumentKey:
                    GraphmlFilename = GetNext(ref i);
                    NothingToDo = false;
                    break;
                case OutputFolderArgumentKey:
                    OutputFolder = GetNext(ref i);
                    NothingToDo = false;
                    break;
                case InitialStateNameArgumentKey:
                    InitialStateName = GetNext(ref i);
                    NothingToDo = false;
                    break;
                case OutputRelativeToFileArgumentKey:
                    IsOutputFolderRelativeToWorkingDir = true;
                    NothingToDo = false;
                    break;
                case UseOriginalStateBaseArgumentKey:
                    UseOriginalStateBase = true;
                    NothingToDo = false;
                    break;
                case InternalArgumentKey:
                    IsInternal = true;
                    NothingToDo = false;
                    break;
                case CustomOptionArgumentKey:
                    FillCustomOption(GetNext(ref i));
                    NothingToDo = false;
                    break;
                case VersionArgumentKey:
                case HelpArgumentKey:
                    break;
                default:
                    errors.Add(string.Format("Unknown '{0}' argument.", args[i]));
                    break;
            }
        }

        if (NothingToDo == false && string.IsNullOrWhiteSpace(GraphmlFilename))
            errors.Add(string.Format("Input file is not set but is mandatory. Please use {0} argument.", GraphmlFilenameArgumentKey));
    }

    private string GetNext(ref int currrent)
    {
        if (currrent + 1 < args.Length)
        {
            var result = args[currrent + 1];
            currrent++;
            return result;
        }

        throw new ArgumentException("Missing option value in command line.");
    }

    private void FillCustomOption(string raw)
    {
        string[] keyValue = raw.Split(new char[] { '=' }, 2);

        if (keyValue.Length == 1)
            customOptions.Add(keyValue[0], null);
        else
            customOptions.Add(keyValue[0], keyValue[1]);
    }

    public void PrintVersion()
    {
        Console.WriteLine("StateMachineTool v" + Assembly.GetExecutingAssembly().GetName().Version);
    }

    public void PrintUsage()
    {
        PrintCommand(VersionArgumentKey, "Shows current version number.");
        PrintCommand(HelpArgumentKey, "Shows this help.");
        Console.WriteLine();

        BeginRequired();
        PrintCommand(GraphmlFilenameArgumentKey, "<file> sets the input graph description file.");
        EndRequired();
        Console.WriteLine();

        PrintCommand(NamespaceNameArgumentKey, "<namespace> sets the namespace of generated files.");
        PrintAdditionalInfo("If not set, the classes are generated without namespace.");
        Console.WriteLine();

        PrintCommand(StateMachineNameArgumentKey, "<name> sets the name of the state machine.");
        PrintAdditionalInfo("It is used to prefix some classes or other code elements.");
        PrintAdditionalInfo("If it is not set, the name defined in the graph file is used.");
        PrintAdditionalInfo("When both are not defined, an error is displayed and code");
        PrintAdditionalInfo("generation is aborted.");
        Console.WriteLine();

        PrintCommand(OutputFolderArgumentKey, "<folder> sets the output folder where code is generated.");
        PrintAdditionalInfo("If it is not set, the output folder is the folder where the graph");
        PrintAdditionalInfo("file is located.");
        Console.WriteLine();

        PrintCommand(OutputRelativeToFileArgumentKey, string.Format("If {0} parameter is used, then:", OutputFolderArgumentKey));
        PrintAdditionalInfo(string.Format("    If <folder> is absolute, the flag {0} is ignored.", OutputRelativeToFileArgumentKey));
        PrintAdditionalInfo("    If <folder> is relative, then:");
        PrintAdditionalInfo(string.Format("        If {0} flag is not set, then <folder> is relative to", OutputRelativeToFileArgumentKey));
        PrintAdditionalInfo("          the graph file directory.");
        PrintAdditionalInfo(string.Format("If {0} parameter is not used, then:", OutputFolderArgumentKey));
        PrintAdditionalInfo(string.Format("    If {0} flag is set, then the output folder is the", OutputRelativeToFileArgumentKey));
        PrintAdditionalInfo("      current working directory.");
        PrintAdditionalInfo(string.Format("    If {0} flag is not set, then the output folder is the", OutputRelativeToFileArgumentKey));
        PrintAdditionalInfo("      graph file directory.");
        Console.WriteLine();

        PrintCommand(InitialStateNameArgumentKey, "<state> generates the SetInitialState(<state>) call.");
        PrintAdditionalInfo("If not set, the IsInitialState flag from");
        PrintAdditionalInfo("the .graphml file is used, if any.");
        Console.WriteLine();

        PrintCommand(UseOriginalStateBaseArgumentKey, "Makes all generated state classes to inherit from");
        PrintAdditionalInfo("Bitcraft.StateMachine.StateBase class instead of from");
        PrintAdditionalInfo("<name>StateBase.");
        Console.WriteLine();

        PrintCommand(InternalArgumentKey, "Makes all exposed types internal instead of public");
        Console.WriteLine();

        PrintCommand(CustomOptionArgumentKey, "<key>=<value> creates a custom key/value pair.");
        PrintAdditionalInfo("Custom key/values are transmitted as is to source code generators");
        PrintAdditionalInfo("for them to decide to ignore them or interpret them as they want.");
        Console.WriteLine();
    }

    private ConsoleColor color;

    private void BeginRequired()
    {
        color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
    }

    private void EndRequired()
    {
        Console.ForegroundColor = color;
    }

    private void PrintCommand(string key, string description)
    {
        Console.WriteLine("{0,12} {1}", key, description);
    }

    private void PrintAdditionalInfo(string description)
    {
        Console.WriteLine("{0,12} {1}", "", description);
    }
}
