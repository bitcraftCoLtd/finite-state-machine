using Bitcraft.StateMachineTool.Core;
using Bitcraft.ToolKit.CodeGeneration;
using System.Text;

namespace Bitcraft.StateMachineTool;

public readonly struct GeneratorOptions
{
    public required string StateMachineName { get; init; }
    public required string OutputPath { get; init; }
    public required string? NamespaceName { get; init; }
    public required bool IsInternal { get; init; }
    public required IGraph Graph { get; init; }
    public required INode? InitialNode { get; init; }
    public required bool UseOriginalStateBase { get; init; }
    public required IReadOnlyDictionary<string, string?> CustomOptions { get; init; }
}

public interface IGenerator
{
    void Generate(GeneratorOptions options);
}

public static class Utils
{
    public static void WriteFile(ICodeGenerator codeGenerator, string basePath, string relativeFilename)
    {
        var sb = new StringBuilder();
        codeGenerator.Write(new CodeWriter(sb, new string(' ', 4), LineEnding.LF));

        string targetFilename = Path.Combine(basePath, relativeFilename);
        string? targetDirectory = Path.GetDirectoryName(targetFilename);

        if (targetDirectory == null)
            throw new IOException($"Invalid target filename '{targetFilename}'.");

        if (Directory.Exists(targetDirectory) == false)
            Directory.CreateDirectory(targetDirectory);

        File.WriteAllText(targetFilename, sb.ToString());
    }
}
