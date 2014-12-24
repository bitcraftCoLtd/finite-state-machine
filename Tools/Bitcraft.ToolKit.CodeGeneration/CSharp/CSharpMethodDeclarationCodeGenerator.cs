using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration.CSharp
{
    public class CSharpMethodDeclarationCodeGenerator : MethodDeclarationCodeGenerator
    {
        public CSharpMethodDeclarationCodeGenerator(ILanguageAbstraction languageAbstraction, AccessModifier accessModifier, bool isStatic, string[] additionalModifiers, string returnType, string name, ArgumentInfo[] arguments, ScopeCodeGenerator bodyGenerator)
            : base(languageAbstraction, accessModifier, isStatic, additionalModifiers, returnType, name, arguments, bodyGenerator)
        {
        }

        protected virtual void ConstructModifiers(List<string> outModifiers)
        {
            var accessModifierStr = CSharpCodeGenerationUtility.AccessModifierToString(accessModifier);
            if (accessModifierStr != null)
                outModifiers.Add(accessModifierStr);

            if (isStatic)
                outModifiers.Add("static");

            if (additionalModifiers != null)
            {
                foreach (var m in additionalModifiers.Where(x => string.IsNullOrWhiteSpace(x) == false))
                    outModifiers.Add(m);
            }
        }

        protected virtual void ConstructArguments(List<string> outArguments)
        {
            if (arguments != null)
            {
                foreach (var a in arguments.Where(p => p.Name != null))
                    outArguments.Add(string.Format("{0} {1}", a.Type, a.Name));
            }
        }

        public override void Write(CodeWriter writer)
        {
            WritePrototype(writer);
            WriteBody(writer);
        }

        protected virtual void WritePrototype(CodeWriter writer)
        {
            var modifiers = new List<string>();
            ConstructModifiers(modifiers);

            var arguments = new List<string>();
            ConstructArguments(arguments);

            var proto = string.Format("{0}{1} {2}({3})",
                string.Join(" ", modifiers),
                returnType != null ? " " + returnType : "",
                name,
                string.Join(", ", arguments));

            writer.Append(proto);
        }

        protected virtual void WriteBody(CodeWriter writer)
        {
            if (bodyGenerator != null)
            {
                using (writer.SuspendIndentation())
                    writer.AppendLine();
                bodyGenerator.Write(writer);
            }
            else
            {
                using (writer.SuspendIndentation())
                    writer.AppendLine(";");
            }
        }
    }
}
