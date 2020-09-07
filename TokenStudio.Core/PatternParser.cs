using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TokenStudio.Core
{
    public class PatternParser
    {
        private readonly IReadOnlyList<(string Name, Regex Regex)> patterns;
        private readonly string input;
        private int currentPosition = 0;

        public PatternParser(IEnumerable<PatternInfo> patterns, string input)
        {
            this.patterns = patterns.Select(pattern => (pattern.Name, new Regex(pattern.Pattern))).ToList();
            this.input = input;
        }

        public Token GetToken()
        {
            if (currentPosition >= input.Length)
                return null;

            var (name, match) = patterns
                .Select(o => (o.Name, o.Regex.Match(input, currentPosition)))
                .FirstOrDefault(o => o.Item2.Success);

            if(name == default)
            {
                currentPosition = input.Length;
                return null;
            }

            if(match.Index > currentPosition)
            {
                var token = new Token(default, default, input.Substring(currentPosition, match.Index - currentPosition));
                currentPosition = match.Index;
                return token;
            }

            return new Token(name, match, match.Value);
        }

        public class Token
        {
            public string Name { get; }

            public string Value { get; }

            public Match Match { get; }

            internal Token(string name, Match match, string value)
            {
                Name = name;
                Match = match;
                Value = value;
            }
        }
    }
}
