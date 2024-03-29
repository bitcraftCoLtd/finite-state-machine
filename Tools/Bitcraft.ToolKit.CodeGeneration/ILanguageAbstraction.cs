﻿namespace Bitcraft.ToolKit.CodeGeneration;

public interface ILanguageAbstraction
{
    NamespaceCodeGenerator CreateNamespaceCodeGenerator(string namespaceName, bool closeWithNewLine, ScopeCodeGenerator bodyGenerator);
    UsingCodeGenerator CreateUsingCodeGenerator(params string[] usings);

    ClassCodeGenerator CreateClassCodeGenerator(AccessModifier accessModifier, string[]? additionalModifiers, string name, string[]? bases, ScopeCodeGenerator bodyGenerator);
    CommentCodeGenerator CreateCommentCodeGenerator(ICodeGenerator innerGenerator, bool isSingleLine);

    ConstructorDeclarationCodeGenerator CreateConstructorDeclarationCodeGenerator(AccessModifier accessModifier, bool isStatic, string? className, string name, ArgumentInfo[]? arguments, ParentConstructorInfo? parentConstructorInfo, string[]? parentConstructorParameters, ScopeCodeGenerator bodyGenerator);

    VariableDeclarationCodeGenerator CreateVariableDeclarationCodeGenerator(AccessModifier accessModifier, string[] additionalModifiers, string type, string name, string initializationStatement);
    VariableDeclarationCodeGenerator CreateVariableDeclarationCodeGenerator(AccessModifier accessModifier, string[] additionalModifiers, string type, string name, ICodeGenerator innerGenerator);

    MethodCallCodeGenerator CreateMethodCallCodeGenerator(string name, params string[] parameters);
    MethodDeclarationCodeGenerator CreateMethodDeclarationCodeGenerator(AccessModifier accessModifier, bool isStatic, string[]? additionalModifiers, string returnType, string className, string name, ArgumentInfo[]? arguments, ScopeCodeGenerator? bodyGenerator);

    ScopeCodeGenerator CreateScopeCodeGenerator(ICodeGenerator? innerGenerator, ScopeContentType scopeContentType, bool closeWithNewLine);
    RawStatementCodeGenerator CreateRawStatementCodeGenerator(string rawStatement);
}
