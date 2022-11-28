using Bitcraft.StateMachineTool.Core;
using Bitcraft.StateMachineTool.Cpp;
using Bitcraft.StateMachineTool.CSharp;
using System;
using System.IO;
using System.Linq;

namespace Bitcraft.StateMachineTool
{
    class Program
    {
        static int Main(string[] args)
        {
            return new Program().Run(new CommandArguments(args));
        }

        private int Run(CommandArguments args)
        {
            if (args.Errors.Count > 0)
            {
                foreach (var err in args.Errors)
                    Console.WriteLine(err);
                return -1;
            }

            if (args.NothingToDo)
                return 0;

            string graphmlAbsoluteFilename = args.GraphmlFilename;
            if (Path.IsPathRooted(graphmlAbsoluteFilename) == false)
                graphmlAbsoluteFilename = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, graphmlAbsoluteFilename));

            string outputPath = args.OutputFolder;
            if (string.IsNullOrWhiteSpace(outputPath) == false)
            {
                if (Path.IsPathRooted(outputPath) == false)
                {
                    var basePath = args.IsOutputFolderRelativeToWorkingDir
                        ? Environment.CurrentDirectory
                        : Path.GetDirectoryName(graphmlAbsoluteFilename);
                    outputPath = Path.GetFullPath(Path.Combine(basePath, outputPath));
                }
            }
            else
            {
                if (args.IsOutputFolderRelativeToWorkingDir)
                    outputPath = Environment.CurrentDirectory;
                else
                    outputPath = Path.GetDirectoryName(graphmlAbsoluteFilename);
            }

            Directory.CreateDirectory(outputPath);

            IParser parser = new Bitcraft.StateMachineTool.yWorks.Parser();

            IGraph graph = null;
            using (var stream = new FileStream(graphmlAbsoluteFilename, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    graph = parser.Parse(stream);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Graph file parse error:");
                    Console.WriteLine(ex);
                    return -1;
                }
            }

            string namespaceName = args.NamespaceName;
            string stateMachineName = args.StateMachineName ?? graph.Semantic; // command line argument has priority
            bool isInternal = args.IsInternal;

            var graphInitialNodeName = graph.InitialNode != null ? graph.InitialNode.Semantic : null;

            INode initialNode = null;
            if (string.IsNullOrWhiteSpace(args.InitialStateName) == false)
            {
                var state = graph.Nodes.SingleOrDefault(x => x.Semantic == args.InitialStateName);
                if (state == null)
                {
                    Console.WriteLine($"State '{args.InitialStateName}' not found.");
                    return -1;
                }

                initialNode = state;
            }
            else
            {
                initialNode = graph.InitialNode;
            }

            if (string.IsNullOrWhiteSpace(stateMachineName))
            {
                var format = "State machine name is missing but is mandatory. Please set it either in the graphml file or through the {0} argument";
                Console.WriteLine(string.Format(format, CommandArguments.StateMachineNameArgumentKey));
                return -1;
            }

            var options = new GeneratorOptions
            {
                StateMachineName = stateMachineName,
                OutputPath = outputPath,
                IsInternal = isInternal,
                NamespaceName = namespaceName,
                Graph = graph,
                InitialNode = initialNode,
                UseOriginalStateBase = args.UseOriginalStateBase,
            };

            IGenerator generator = new CSharpGenerator();
            generator.Generate(options);

            return 0;
        }
    }
}
