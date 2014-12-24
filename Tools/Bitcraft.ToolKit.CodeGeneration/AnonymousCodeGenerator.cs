using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration
{
    public class AnonymousCodeGenerator : ICodeGenerator
    {
        public ILanguageAbstraction Language { get; private set; }
        private Action<CodeWriter> codeGenerator;

        public AnonymousCodeGenerator(ILanguageAbstraction languageAbstraction, Action<CodeWriter> codeGenerator)
        {
            if (languageAbstraction == null)
                throw new ArgumentNullException("languageAbstraction");
            if (codeGenerator == null)
                throw new ArgumentNullException("codeGenerator");

            Language = languageAbstraction;
            this.codeGenerator = codeGenerator;
        }

        public void Write(CodeWriter writer)
        {
            codeGenerator(writer);
        }
    }
}
