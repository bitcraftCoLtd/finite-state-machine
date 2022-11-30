namespace Bitcraft.ToolKit.CodeGeneration.CSharp;

public class CSharpLanguageAbstraction : ILanguageAbstraction
{
    public NamespaceCodeGenerator CreateNamespaceCodeGenerator(string namespaceName, bool closeWithNewLine, ScopeCodeGenerator bodyGenerator)
    {
        return new CSharpNamespaceCodeGenerator(this, namespaceName, closeWithNewLine, bodyGenerator);
    }

    public UsingCodeGenerator CreateUsingCodeGenerator(params string[] usings)
    {
        return new CSharpUsingCodeGenerator(this, usings);
    }

    public ClassCodeGenerator CreateClassCodeGenerator(AccessModifier accessModifier, string[]? additionalModifiers, string name, string[]? bases, ScopeCodeGenerator bodyGenerator)
    {
        return new CSharpClassCodeGenerator(this, accessModifier, additionalModifiers, name, bases, bodyGenerator);
    }

    public CommentCodeGenerator CreateCommentCodeGenerator(ICodeGenerator innerGenerator, bool isSingleLine)
    {
        return new CSharpCommentCodeGenerator(this, innerGenerator, isSingleLine);
    }

    public ConstructorDeclarationCodeGenerator CreateConstructorDeclarationCodeGenerator(AccessModifier accessModifier, bool isStatic, string? className, string name, ArgumentInfo[]? arguments, ParentConstructorInfo? parentConstructorInfo, string[]? parentConstructorParameters, ScopeCodeGenerator bodyGenerator)
    {
        return new CSharpConstructorDeclarationCodeGenerator(this, accessModifier, isStatic, className, name, arguments, parentConstructorInfo, parentConstructorParameters, bodyGenerator);
    }

    public VariableDeclarationCodeGenerator CreateVariableDeclarationCodeGenerator(AccessModifier accessModifier, string[] additionalModifiers, string type, string name, string initializationStatement)
    {
        return new CSharpVariableDeclarationCodeGenerator(this, accessModifier, additionalModifiers, type, name, initializationStatement);
    }

    public VariableDeclarationCodeGenerator CreateVariableDeclarationCodeGenerator(AccessModifier accessModifier, string[] additionalModifiers, string type, string name, ICodeGenerator innerGenerator)
    {
        return new CSharpVariableDeclarationCodeGenerator(this, accessModifier, additionalModifiers, type, name, innerGenerator);
    }

    public MethodCallCodeGenerator CreateMethodCallCodeGenerator(string name, params string[] parameters)
    {
        return new CSharpMethodCallCodeGenerator(this, name, parameters);
    }

    public MethodDeclarationCodeGenerator CreateMethodDeclarationCodeGenerator(AccessModifier accessModifier, bool isStatic, string[]? additionalModifiers, string returnType, string className, string name, ArgumentInfo[]? arguments, ScopeCodeGenerator? bodyGenerator)
    {
        return new CSharpMethodDeclarationCodeGenerator(this, accessModifier, isStatic, additionalModifiers, returnType, className, name, arguments, bodyGenerator);
    }

    public ScopeCodeGenerator CreateScopeCodeGenerator(ICodeGenerator? innerGenerator, ScopeContentType scopeContentType, bool closeWithNewLine)
    {
        return new CSharpScopeCodeGenerator(this, scopeContentType, innerGenerator, closeWithNewLine);
    }

    public RawStatementCodeGenerator CreateRawStatementCodeGenerator(string rawStatement)
    {
        return new CSharpRawStatementCodeGenerator(this, rawStatement);
    }
}
