using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TokenStudio.Core
{
    public class TokenParser<TTokenType>
        where TTokenType : struct, Enum
    {
        private readonly Dictionary<TTokenType, Regex> patterns = new Dictionary<TTokenType, Regex>();

        public void AddPattern(TTokenType tokenType, string pattern)
        {
            if(patterns.ContainsKey(tokenType))
            {
                throw new InvalidOperationException($"{tokenType} is already registered.");
            }

            patterns[tokenType] = new Regex(pattern);
        }

        public IEnumerable<Token> GetTokenStream(string input)
        {
            var currentPosition = 0;
            while(currentPosition < input.Length)
            {
                var (type, match) = patterns
                    .Select(pair => (Type: pair.Key, Match: pair.Value.Match(input, currentPosition)))
                    .FirstOrDefault(pair => pair.Match.Success);

                if(match == default)
                {
                    yield return new Token(default, default, input.Substring(currentPosition));
                    currentPosition = input.Length;
                }
                else if(match.Index > currentPosition)
                {
                    yield return new Token(default, default, input.Substring(currentPosition, match.Index - currentPosition));
                }

                yield return new Token(type, match, match.Value);
                currentPosition = match.Index + match.Length;
            }
        }

        public class Token
        {
            public Token(TTokenType? type, Match match, string value)
            {
                Type = type;
                Match = match;
                Value = value;
            }

            public TTokenType? Type { get; }

            public Match Match { get; }

            public string Value { get; }
        }
    }
}
