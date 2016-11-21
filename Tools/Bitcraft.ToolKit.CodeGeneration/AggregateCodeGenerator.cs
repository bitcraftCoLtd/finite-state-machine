using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration
{
    public class AggregateCodeGenerator : ICodeGenerator
    {
        public ILanguageAbstraction Language { get; }
        private IEnumerable<ICodeGenerator> codeGenerators;

        public AggregateCodeGenerator(ILanguageAbstraction languageAbstraction, params ICodeGenerator[] codeGenerators)
            : this(languageAbstraction, (IEnumerable<ICodeGenerator>)codeGenerators)
        {
        }

        public AggregateCodeGenerator(ILanguageAbstraction languageAbstraction, IEnumerable<ICodeGenerator> codeGenerators)
        {
            if (languageAbstraction == null)
                throw new ArgumentNullException(nameof(languageAbstraction));
            if (codeGenerators == null)
                throw new ArgumentNullException(nameof(codeGenerators));

            Language = languageAbstraction;
            this.codeGenerators = codeGenerators;
        }

        public void Write(CodeWriter writer)
        {
            foreach (var gen in codeGenerators)
                gen.Write(writer);
        }
    }
}
