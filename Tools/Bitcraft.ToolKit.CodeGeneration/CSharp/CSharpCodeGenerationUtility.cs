namespace Bitcraft.ToolKit.CodeGeneration.CSharp;

public static class CSharpCodeGenerationUtility
{
    public static string? AccessModifierToString(AccessModifier accessModifier)
    {
        switch (accessModifier)
        {
            case AccessModifier.Public:
                return "public";
            case AccessModifier.Protected:
                return "protected";
            case AccessModifier.ProtectedInternal:
                return "protected internal";
            case AccessModifier.Internal:
                return "internal";
            case AccessModifier.Private:
                return "private";
        }

        return null;
    }
}
