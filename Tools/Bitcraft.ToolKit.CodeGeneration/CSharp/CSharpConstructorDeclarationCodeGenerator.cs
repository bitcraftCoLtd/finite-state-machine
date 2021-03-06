﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration.CSharp
{
    public class CSharpConstructorDeclarationCodeGenerator : ConstructorDeclarationCodeGenerator
    {
        public CSharpConstructorDeclarationCodeGenerator(
            ILanguageAbstraction languageAbstraction,
            AccessModifier accessModifier,
            bool isStatic,
            string name,
            ArgumentInfo[] arguments,
            ParentConstructorType parentConstructorType,
            string[] parentConstructorParameters,
            ScopeCodeGenerator bodyGenerator)
            : base(languageAbstraction, accessModifier, isStatic, name, arguments, parentConstructorType, parentConstructorParameters, bodyGenerator)
        {
        }

        protected virtual void ConstructModifiers(List<string> outModifiers)
        {
            var accessModifierStr = CSharpCodeGenerationUtility.AccessModifierToString(accessModifier);

            if (isStatic)
                outModifiers.Add("static");
            else if (accessModifierStr != null)
                outModifiers.Add(accessModifierStr);

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

            if (parentConstructorType != ParentConstructorType.None)
            {
                var parentConstructor = parentConstructorType == ParentConstructorType.Base
                    ? "base"
                    : "this";

                // prototype is not newline ended
                writer.AppendLine();

                using (writer.Indent())
                {
                    writer.Append(": {0}({1})",
                        parentConstructor,
                        string.Join(", ",
                        (parentConstructorParameters ?? new string[0])
                            .Where(x => string.IsNullOrWhiteSpace(x) == false)
                            .Select(x => x.Trim())));
                }
            }

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
