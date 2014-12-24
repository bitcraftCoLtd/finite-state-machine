using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration
{
    public abstract class CommentCodeGenerator : ICodeGenerator
    {
        public ILanguageAbstraction Language { get; private set; }
        protected readonly ICodeGenerator innerGenerator;
        protected readonly bool isSingleLine;

        public CommentCodeGenerator(ILanguageAbstraction languageAbstraction, ICodeGenerator innerGenerator, bool isSingleLine)
        {
            if (languageAbstraction == null)
                throw new ArgumentNullException("languageAbstraction");
            if (innerGenerator == null)
                throw new ArgumentNullException("innerGenerator");

            Language = languageAbstraction;

            this.innerGenerator = innerGenerator;
            this.isSingleLine = isSingleLine;
        }

        public abstract void Write(CodeWriter writer);
    }
}
