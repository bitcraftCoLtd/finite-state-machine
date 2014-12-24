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
        public ILanguageAbstraction Language { get; private set; }
        protected string namespaceName { get; private set; }
        protected string stateMachineName { get; private set; }

        public CodeGeneratorBase(ILanguageAbstraction languageAbstraction, string namespaceName, string stateMachineName)
        {
            if (languageAbstraction == null)
                throw new ArgumentNullException("languageAbstraction");
            if (string.IsNullOrWhiteSpace(namespaceName) == false)
                this.namespaceName = namespaceName;

            CodeGenerationUtility.CheckValidIdentifierArgument(stateMachineName, "stateMachineName");

            Language = languageAbstraction;
            this.stateMachineName = stateMachineName;
        }

        public virtual void Write(CodeWriter writer)
        {
            var writeContentCodeGenerator = new AnonymousCodeGenerator(Language, WriteContent);

            if (namespaceName != null)
            {
                Language.CreateNamespaceCodeGenerator(namespaceName).Write(writer);
                Language.CreateScopeCodeGenerator(writeContentCodeGenerator, true).Write(writer);
            }
            else
                writeContentCodeGenerator.Write(writer);
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
