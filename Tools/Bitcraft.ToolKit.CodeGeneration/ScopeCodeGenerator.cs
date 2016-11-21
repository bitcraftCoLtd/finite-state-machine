using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration
{
    public abstract class ScopeCodeGenerator : ICodeGenerator
    {
        public ILanguageAbstraction Language { get; }
        protected readonly ICodeGenerator innerGenerator;
        protected readonly bool closeWithNewLine;

        public ScopeCodeGenerator(ILanguageAbstraction languageAbstraction, ICodeGenerator innerGenerator)
            : this(languageAbstraction, innerGenerator, true)
        {
        }

        public ScopeCodeGenerator(ILanguageAbstraction languageAbstraction, ICodeGenerator innerGenerator, bool closeWithNewLine)
        {
            Language = languageAbstraction;

            this.innerGenerator = innerGenerator;
            this.closeWithNewLine = closeWithNewLine;
        }

        public abstract void Write(CodeWriter writer);
    }
}
