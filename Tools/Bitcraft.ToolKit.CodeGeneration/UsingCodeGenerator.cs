using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration
{
    public abstract class UsingCodeGenerator : ICodeGenerator
    {
        public ILanguageAbstraction Language { get; private set; }

        protected readonly string[] usings;

        public UsingCodeGenerator(ILanguageAbstraction languageAbstraction, params string[] usings)
        {
            if (languageAbstraction == null)
                throw new ArgumentNullException("languageAbstraction");
            this.usings = usings;

            Language = languageAbstraction;
        }

        public abstract void Write(CodeWriter writer);
    }
}
