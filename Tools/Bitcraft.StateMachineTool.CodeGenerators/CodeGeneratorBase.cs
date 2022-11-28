using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitcraft.ToolKit.CodeGeneration;

namespace Bitcraft.StateMachineTool.CodeGenerators
{
    public abstract class CodeGeneratorBase : ICodeGenerator
    {
        public ILanguageAbstraction Language { get; }
        protected string namespaceName { get; }
        protected string stateMachineName { get; }

        public CodeGeneratorBase(ILanguageAbstraction languageAbstraction, string namespaceName, string stateMachineName)
        {
            if (languageAbstraction == null)
                throw new ArgumentNullException(nameof(languageAbstraction));
            if (string.IsNullOrWhiteSpace(namespaceName) == false)
                this.namespaceName = namespaceName;

            CodeGenerationUtility.CheckValidIdentifierArgument(stateMachineName, nameof(stateMachineName));

            Language = languageAbstraction;
            this.stateMachineName = stateMachineName;
        }

        public virtual void Write(CodeWriter writer)
        {
            var contentCodeGenerator = new AnonymousCodeGenerator(Language, WriteContent);

            if (namespaceName == null)
            {
                contentCodeGenerator.Write(writer);
                return;
            }

            ScopeCodeGenerator nsCodeGenerator = Language.CreateScopeCodeGenerator(contentCodeGenerator, ScopeContentType.Namespace, false);
            Language.CreateNamespaceCodeGenerator(namespaceName, true, nsCodeGenerator).Write(writer);
        }

        protected abstract void WriteContent(CodeWriter writer);

        protected void WriteFileHeader(CodeWriter writer)
        {
            Language.CreateCommentCodeGenerator(new AnonymousCodeGenerator(Language, w =>
                {
                    writer.AppendLine(" ###########################################################");
                    writer.AppendLine(" ### Warning: this file has been generated automatically ###");
                    writer.AppendLine(" ###                    DO NOT MODIFY                    ###");
                    writer.AppendLine(" ###########################################################");
                }), false).Write(writer);
            writer.AppendLine();
        }
    }
}
