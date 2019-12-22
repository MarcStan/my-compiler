using CodeAnalysis.Text;
using FluentAssertions;
using NUnit.Framework;

namespace CodeAnalysis.Tests
{
    public class SourceTextTests
    {
        [TestCase(".", 1)]
        [TestCase(".\r\n", 2)]
        [TestCase(".\r\n\r\n", 3)]
        public void LastLine_should_not_be_dropped_from_input(string text, int expectedLines)
        {
            var src = SourceText.From(text);
            src.Lines.Should().HaveCount(expectedLines);
        }
    }
}
