using System.Text.RegularExpressions;

namespace Bitcraft.ToolKit.CodeGeneration;

public static partial class CodeGenerationUtility
{
    private static readonly Regex identifierRegex = IdentifierRegex();
    private static readonly Regex partialIdentifierRegex = PartialIdentifierRegex();

    public static bool IsValidIdentifier(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        return identifierRegex.Match(name).Success;
    }

    public static bool IsValidPartialIdentifier(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        return partialIdentifierRegex.Match(name).Success;
    }

    public static void CheckValidPartialIdentifierArgument(string identifier, string paramName)
    {
        if (string.IsNullOrWhiteSpace(paramName))
            throw new InvalidOperationException($"Invalid '{nameof(paramName)}' argument.");

        if (IsValidPartialIdentifier(identifier) == false)
            throw new ArgumentException(string.Format("Invalid '{0}' argument. It must follow the C# identifier naming rules", paramName));
    }

    public static void CheckValidIdentifierArgument(string identifier, string paramName)
    {
        if (string.IsNullOrWhiteSpace(paramName))
            throw new InvalidOperationException($"Invalid '{nameof(paramName)}' argument.");

        //if (IsValidIdentifier(identifier) == false)
        //    throw new ArgumentException(string.Format("Invalid '{0}' argument. It must follow the C# identifier naming rules", paramName));
    }

    public static void CheckNullOrWhitespaceArgument(string str, string paramName)
    {
        if (string.IsNullOrWhiteSpace(paramName))
            throw new InvalidOperationException($"Invalid '{nameof(paramName)}' argument.");

        if (string.IsNullOrWhiteSpace(str))
            throw new ArgumentException(string.Format("Invalid '{0}' argument. It must not be null or contain only whitespaces", paramName));
    }

    [GeneratedRegex("^[a-zA-z_][a-zA-Z_0-9]*$", RegexOptions.Compiled)]
    private static partial Regex IdentifierRegex();

    [GeneratedRegex("[a-zA-Z_0-9]+", RegexOptions.Compiled)]
    private static partial Regex PartialIdentifierRegex();
}
