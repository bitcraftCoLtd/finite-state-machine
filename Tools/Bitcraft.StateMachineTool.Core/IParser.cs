namespace Bitcraft.StateMachineTool.Core;

public interface IParser
{
    IGraph Parse(Stream stream);
}
