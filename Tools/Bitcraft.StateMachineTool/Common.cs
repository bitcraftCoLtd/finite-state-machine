using Bitcraft.StateMachineTool.Core;
using Bitcraft.ToolKit.CodeGeneration;
using System.IO;
using System.Text;

namespace Bitcraft.StateMachineTool
{
    public struct GeneratorOptions
    {
        public string StateMachineName;
        public string OutputPath;
        public string NamespaceName;
        public bool IsInternal;
        public IGraph Graph;
        public INode InitialNode;
        public bool UseOriginalStateBase;
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
            codeGenerator.Write(new CodeWriter(sb));
            File.WriteAllText(Path.Combine(basePath, relativeFilename), sb.ToString());
        }
    }
}
