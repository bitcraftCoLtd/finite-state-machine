using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration.CSharp
{
    public class CSharpLanguageAbstraction : ILanguageAbstraction
    {
        public NamespaceCodeGenerator CreateNamespaceCodeGenerator(string namespaceName)
        {
            return new CSharpNamespaceCodeGenerator(this, namespaceName);
        }

        public UsingCodeGenerator CreateUsingCodeGenerator(params string[] usings)
        {
            return new CSharpUsingCodeGenerator(this, usings);
        }

        public ClassCodeGenerator CreateClassCodeGenerator(AccessModifier accessModifier, string[] additionalModifiers, string name, string[] bases)
        {
            return new CSharpClassCodeGenerator(this, accessModifier, additionalModifiers, name, bases);
        }

        public CommentCodeGenerator CreateCommentCodeGenerator(ICodeGenerator innerGenerator, bool isSingleLine)
        {
            return new CSharpCommentCodeGenerator(this, innerGenerator, isSingleLine);
        }

        public ConstructorDeclarationCodeGenerator CreateConstructorDeclarationCodeGenerator(AccessModifier accessModifier, bool isStatic, string name, ArgumentInfo[] arguments, ParentConstructorInfo parentConstructorInfo, string[] parentConstructorParameters, ScopeCodeGenerator bodyGenerator)
        {
            return new CSharpConstructorDeclarationCodeGenerator(this, accessModifier, isStatic, name, arguments, parentConstructorInfo, parentConstructorParameters, bodyGenerator);
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

        public MethodDeclarationCodeGenerator CreateMethodDeclarationCodeGenerator(AccessModifier accessModifier, bool isStatic, string[] additionalModifiers, string returnType, string name, ArgumentInfo[] arguments, ScopeCodeGenerator bodyGenerator)
        {
            return new CSharpMethodDeclarationCodeGenerator(this, accessModifier, isStatic, additionalModifiers, returnType, name, arguments, bodyGenerator);
        }

        public ScopeCodeGenerator CreateScopeCodeGenerator(ICodeGenerator innerGenerator, ScopeContentType scopeContentType, bool closeWithNewLine)
        {
            return new CSharpScopeCodeGenerator(this, scopeContentType, innerGenerator, closeWithNewLine);
        }

        public RawStatementCodeGenerator CreateRawStatementCodeGenerator(string rawStatement)
        {
            return new CSharpRawStatementCodeGenerator(this, rawStatement);
        }
    }
}
