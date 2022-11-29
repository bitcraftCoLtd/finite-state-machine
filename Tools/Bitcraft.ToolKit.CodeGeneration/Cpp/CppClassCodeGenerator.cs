using System;
using System.Collections.Generic;
using System.Linq;

namespace Bitcraft.ToolKit.CodeGeneration.Cpp
{
    public class CppClassCodeGenerator : ClassCodeGenerator
    {
        private readonly CppFileType cppFileType;

        public CppClassCodeGenerator(
            ILanguageAbstraction languageAbstraction,
            CppFileType cppFileType,
            AccessModifier accessModifier,
            string[] additionalModifiers,
            string name,
            string[] bases,
            ScopeCodeGenerator bodyGenerator)
            : base(languageAbstraction, accessModifier, additionalModifiers, name, bases, bodyGenerator)
        {
            this.cppFileType = cppFileType;
        }

        private void WriteHeader(CodeWriter writer)
        {
            var elements = new List<string>();

            elements.Add("class");

            elements.Add(name);

            if (bases != null)
            {
                var newBases = bases
                    .Where(x => string.IsNullOrWhiteSpace(x) == false)
                    .Select(x => $"public {x.Trim()}")
                    .ToList();

                if (newBases.Count > 0)
                {
                    elements.Add(":");
                    elements.Add(string.Join(", ", newBases));
                }
            }

            writer.AppendLine(string.Join(" ", elements));
        }

        public override void Write(CodeWriter writer)
        {
            if (cppFileType == CppFileType.Header)
                WriteHeader(writer);

            if (bodyGenerator != null)
            {
                bodyGenerator.Write(writer);

                if (cppFileType == CppFileType.Header)
                {
                    using (writer.SuspendIndentation())
                        writer.Append(";");
                }
            }
        }
    }
}
