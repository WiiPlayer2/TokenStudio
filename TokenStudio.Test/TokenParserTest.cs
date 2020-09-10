using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TokenStudio.Core;

namespace TokenStudio.Test
{
    using TokenStudio.CodeGen;

    [TestClass]
    public partial class TokenParserTest
    {
        [TokenParser]
        private enum TestTokenType
        {
            [Pattern("a+")]
            Token1,

            [Pattern("b+")]
            Token2,
        }

        [TestMethod, Timeout(10000)]
        public void GetTokenStream_WithTestTokenType_ReturnsCorrectTokens()
        {
            var input = "aaabbb";
            var parser = new TokenParser<TestTokenType>();
            parser.AddPattern(TestTokenType.Token1, "a+");
            parser.AddPattern(TestTokenType.Token2, "b+");
            var expectedTokens = new[]
            {
                new {Type = TestTokenType.Token1, Value = "aaa"},
                new {Type = TestTokenType.Token2, Value="bbb"},
            };

            var result = parser.GetTokenStream(input).ToList();

            result.Should().BeEquivalentTo(expectedTokens);
        }

        [TestMethod, Timeout(10000)]
        public void GetTokenStream_WithTestTokenTypeParser_ReturnsCorrectTokens()
        {
            var input = "aaabbb";
            var parser = new TestTokenTypeParser();
            var expectedTokens = new[]
                                 {
                                     new {Type = TestTokenType.Token1, Value = "aaa"},
                                     new {Type = TestTokenType.Token2, Value = "bbb"},
                                 };

            var result = parser.GetTokenStream(input).ToList();

            result.Should().BeEquivalentTo(expectedTokens);
        }
    }
}