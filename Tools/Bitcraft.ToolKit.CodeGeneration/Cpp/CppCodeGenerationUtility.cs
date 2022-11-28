using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration.Cpp
{
    public static class CppCodeGenerationUtility
    {
        public static string AccessModifierToString(AccessModifier accessModifier)
        {
            switch (accessModifier)
            {
                case AccessModifier.Public:
                    return "public";
                case AccessModifier.Protected:
                    return "protected";
                case AccessModifier.Private:
                    return "private";
            }

            return null;
        }
    }
}
