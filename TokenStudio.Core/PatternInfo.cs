using System;
using System.Collections.Generic;
using System.Text;

namespace TokenStudio.Core
{
    public class PatternInfo
    {
        public PatternInfo(string pattern, string name)
        {
            Pattern = pattern;
            Name = name;
        }

        public string Pattern { get; }

        public string Name { get; }
    }
}
