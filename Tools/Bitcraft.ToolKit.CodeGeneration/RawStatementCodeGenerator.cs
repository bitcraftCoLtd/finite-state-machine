using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration
{
    public abstract class RawStatementCodeGenerator : ICodeGenerator
    {
        public ILanguageAbstraction Language { get; }
        protected readonly string rawStatement;

        public RawStatementCodeGenerator(ILanguageAbstraction languageAbstraction, string rawStatement)
        {
            if (languageAbstraction == null)
                throw new ArgumentNullException(nameof(languageAbstraction));
            CodeGenerationUtility.CheckNullOrWhitespaceArgument(rawStatement, nameof(rawStatement));

            Language = languageAbstraction;

            this.rawStatement = rawStatement;
        }

        public abstract void Write(CodeWriter writer);
    }
}
