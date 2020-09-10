using CodeGeneration.Roslyn;
using System;
using System.Diagnostics;

namespace TokenStudio.CodeGen
{
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    [CodeGenerationAttribute("TokenStudio.CodeGen.TokenParserGenerator, TokenStudio.CodeGen")]
    [Conditional("CodeGeneration")]
    public class TokenParserAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class PatternAttribute : Attribute
    {
        public PatternAttribute(string pattern)
        {
            Pattern = pattern;
        }

        public string Pattern { get; }
    }
}
