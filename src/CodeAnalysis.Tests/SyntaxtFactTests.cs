using CodeAnalysis.Syntax;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeAnalysis.Tests
{
    public class SyntaxtFactTests
    {
        [Test]
        [TestCaseSource(nameof(GetSyntaxKindData))]
        public void GetText_should_roundtrip(SyntaxKind kind)
        {
            var text = SyntaxFacts.GetText(kind);

            if (text == null)
                return;

            var tokens = SyntaxTree.ParseTokens(text);
            var token = tokens.Single();
            token.Text.Should().Be(text);
        }

        public static IEnumerable<object[]> GetSyntaxKindData()
        {
            var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            foreach (var k in kinds)
            {
                yield return new object[] { k };
            }
        }
    }
}
