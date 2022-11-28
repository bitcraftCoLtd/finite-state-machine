using System;

namespace Bitcraft.ToolKit.CodeGeneration.Cpp
{
    public class CppLanguageAbstraction : ILanguageAbstraction
    {
        private readonly CppFileType cppFileType;

        public CppLanguageAbstraction(CppFileType cppFileType)
        {
            this.cppFileType = cppFileType;
        }

        public ClassCodeGenerator CreateClassCodeGenerator(AccessModifier accessModifier, string[] additionalModifiers, string name, string[] bases, ScopeCodeGenerator bodyGenerator)
        {
            return new CppClassCodeGenerator(this, cppFileType, accessModifier, additionalModifiers, name, bases, bodyGenerator);
        }

        public CommentCodeGenerator CreateCommentCodeGenerator(ICodeGenerator innerGenerator, bool isSingleLine)
        {
            return new CppCommentCodeGenerator(this, innerGenerator, isSingleLine);
        }

        public ConstructorDeclarationCodeGenerator CreateConstructorDeclarationCodeGenerator(AccessModifier accessModifier, bool isStatic, string name, ArgumentInfo[] arguments, ParentConstructorInfo parentConstructorInfo, string[] parentConstructorParameters, ScopeCodeGenerator bodyGenerator)
        {
            return new CppConstructorDeclarationCodeGenerator(this, cppFileType, accessModifier, isStatic, name, arguments, parentConstructorInfo, parentConstructorParameters, bodyGenerator);
        }

        public MethodCallCodeGenerator CreateMethodCallCodeGenerator(string name, params string[] parameters)
        {
            return new CppMethodCallCodeGenerator(this, cppFileType, name, parameters);
        }

        public MethodDeclarationCodeGenerator CreateMethodDeclarationCodeGenerator(AccessModifier accessModifier, bool isStatic, string[] additionalModifiers, string returnType, string name, ArgumentInfo[] arguments, ScopeCodeGenerator bodyGenerator)
        {
            return new CppMethodDeclarationCodeGenerator(this, cppFileType, accessModifier, isStatic, additionalModifiers, returnType, name, arguments, bodyGenerator);
        }

        public NamespaceCodeGenerator CreateNamespaceCodeGenerator(string namespaceName, bool closeWithNewLine, ScopeCodeGenerator bodyGenerator)
        {
            return new CppNamespaceCodeGenerator(this, cppFileType, namespaceName, closeWithNewLine, bodyGenerator);
        }

        public RawStatementCodeGenerator CreateRawStatementCodeGenerator(string rawStatement)
        {
            return new CppRawStatementCodeGenerator(this, cppFileType, rawStatement);
        }

        public ScopeCodeGenerator CreateScopeCodeGenerator(ICodeGenerator innerGenerator, ScopeContentType scopeContentType, bool closeWithNewLine)
        {
            return new CppScopeCodeGenerator(this, cppFileType, scopeContentType, innerGenerator, closeWithNewLine);
        }

        public UsingCodeGenerator CreateUsingCodeGenerator(params string[] usings)
        {
            return new CppUsingCodeGenerator(this, cppFileType, usings);
        }

        public VariableDeclarationCodeGenerator CreateVariableDeclarationCodeGenerator(AccessModifier accessModifier, string[] additionalModifiers, string type, string name, string initializationStatement)
        {
            return new CppVariableDeclarationCodeGenerator(this, cppFileType, accessModifier, additionalModifiers, type, name, initializationStatement);
        }

        public VariableDeclarationCodeGenerator CreateVariableDeclarationCodeGenerator(AccessModifier accessModifier, string[] additionalModifiers, string type, string name, ICodeGenerator innerGenerator)
        {
            return new CppVariableDeclarationCodeGenerator(this, cppFileType, accessModifier, additionalModifiers, type, name, innerGenerator);
        }
    }
}
