using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using TokenStudio.Core;
using System.ComponentModel.DataAnnotations;

namespace TokenStudio.Test
{
    [TestClass]
    public class PatternParserTest
    {
        [TestMethod]
        public void GetToken_WithPatternMatchingStart_ReturnsStart()
        {
            var input = "aaabbb";
            var patterns = new[]
            {
                new PatternInfo("a+", "Pattern"),
            };
            var expectedToken = new { Name = "Pattern", Value = "aaa" };
            var parser = new PatternParser(patterns, input);

            var token = parser.GetToken();

            token.Should().BeEquivalentTo(expectedToken);
        }
    }
}
