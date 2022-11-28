using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration
{
    public enum ScopeContentType
    {
        Namespace,
        Class,
        Method,
        Statement
    }

    public abstract class ScopeCodeGenerator : ICodeGenerator
    {
        public ILanguageAbstraction Language { get; }
        protected readonly ScopeContentType scopeContentType;
        protected readonly ICodeGenerator innerGenerator;
        protected readonly bool closeWithNewLine;

        public ScopeCodeGenerator(ILanguageAbstraction languageAbstraction, ScopeContentType scopeContentType, ICodeGenerator innerGenerator)
            : this(languageAbstraction, scopeContentType, innerGenerator, true)
        {
        }

        public ScopeCodeGenerator(ILanguageAbstraction languageAbstraction, ScopeContentType scopeContentType, ICodeGenerator innerGenerator, bool closeWithNewLine)
        {
            Language = languageAbstraction;

            this.scopeContentType = scopeContentType;
            this.innerGenerator = innerGenerator;
            this.closeWithNewLine = closeWithNewLine;
        }

        public abstract void Write(CodeWriter writer);
    }
}
