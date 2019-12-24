using CodeAnalysis.Syntax;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeAnalysis.Tests
{
    public class LexerTests
    {
        [Test]
        public void Ensure_all_tokens_are_tested()
        {
            var tokenKinds = Enum.GetValues(typeof(SyntaxKind))
                .Cast<SyntaxKind>()
                .Where(k => k.ToString().EndsWith("Keyword") ||
                            k.ToString().EndsWith("Token"))
                .ToList();

            var testedTokens = GetTokens()
                .Concat(GetSeparators())
                .Select(x => x.kind)
                .ToList();

            var untested = tokenKinds
                .Except(testedTokens)
                .Except(new[] { SyntaxKind.EndOfFileToken, SyntaxKind.BadToken })
                .ToList();

            untested.Should().BeEmpty();
        }

        [Test]
        [TestCaseSource(nameof(GetTokenData))]
        public void Single_token_lexing_should_work(SyntaxKind kind, string text)
        {
            var tokens = SyntaxTree.ParseTokens(text);

            tokens.Should().HaveCount(1);
            var token = tokens.Single();

            token.Kind.Should().Be(kind);
            token.Text.Should().Be(text);
        }

        [Test]
        [TestCaseSource(nameof(GetTokenPairData))]
        public void Multi_token_lexing_should_work(SyntaxKind kind1, string text1, SyntaxKind kind2, string text2)
        {
            var tokens = SyntaxTree.ParseTokens(text1 + text2);

            tokens.Should().HaveCount(2);
            var token1 = tokens.First();
            var token2 = tokens.Skip(1).First();

            token1.Kind.Should().Be(kind1);
            token1.Text.Should().Be(text1);
            token2.Kind.Should().Be(kind2);
            token2.Text.Should().Be(text2);
        }

        [Test]
        [TestCaseSource(nameof(GetTokenPairWithSeparatorData))]
        public void Multi_token_lexing_should_work_with_separators(SyntaxKind kind1, string text1, SyntaxKind separatorKind, string separatorText, SyntaxKind kind2, string text2)
        {
            var tokens = SyntaxTree.ParseTokens(text1 + separatorText + text2);

            tokens.Should().HaveCount(3);
            var token1 = tokens.First();
            var token2 = tokens.Skip(1).First();
            var token3 = tokens.Skip(2).First();

            token1.Kind.Should().Be(kind1);
            token1.Text.Should().Be(text1);
            token2.Kind.Should().Be(separatorKind);
            token2.Text.Should().Be(separatorText);
            token3.Kind.Should().Be(kind2);
            token3.Text.Should().Be(text2);
        }

        private static IEnumerable<object[]> GetTokenData()
            => GetTokens().Concat(GetSeparators()).Select(x => new object[] { x.kind, x.text });

        private static IEnumerable<object[]> GetTokenPairData()
            => GetTokenPairs().Select(x => new object[] { x.t1Kind, x.t1Text, x.t2Kind, x.t2Text });

        private static IEnumerable<object[]> GetTokenPairWithSeparatorData()
            => GetTokenPairsWithSeparators().Select(x => new object[] { x.t1Kind, x.t1Text, x.separatorKind, x.separatorText, x.t2Kind, x.t2Text });

        private static IEnumerable<(SyntaxKind kind, string text)> GetTokens()
        {
            var fixedTokens = Enum.GetValues(typeof(SyntaxKind))
                .Cast<SyntaxKind>()
                .Select(k => (kind: k, text: SyntaxFacts.GetText(k)))
                .Where(t => t.text != null);

            var dynamicTokens = new[]
            {
                (SyntaxKind.NumberToken, "1"),
                (SyntaxKind.NumberToken, "123"),
                (SyntaxKind.IdentifierToken, "a"),
                (SyntaxKind.IdentifierToken, "abc")
            };

            return fixedTokens.Concat(dynamicTokens);
        }

        private static IEnumerable<(SyntaxKind kind, string text)> GetSeparators()
        {
            return new[]
            {
                (SyntaxKind.Whitespace, " "),
                (SyntaxKind.Whitespace, "  "),
                (SyntaxKind.Whitespace, "\r"),
                (SyntaxKind.Whitespace, "\n"),
                (SyntaxKind.Whitespace, "\r\n")
            };
        }

        private static IEnumerable<(SyntaxKind t1Kind, string t1Text, SyntaxKind t2Kind, string t2Text)> GetTokenPairs()
        {
            foreach (var t1 in GetTokens())
                foreach (var t2 in GetTokens())
                {
                    if (!RequiresSeparator(t1.kind, t2.kind))
                        yield return (t1.kind, t1.text, t2.kind, t2.text);
                }
        }

        private static IEnumerable<(
            SyntaxKind t1Kind, string t1Text,
            SyntaxKind separatorKind, string separatorText,
            SyntaxKind t2Kind, string t2Text)> GetTokenPairsWithSeparators()
        {
            foreach (var t1 in GetTokens())
                foreach (var t2 in GetTokens())
                {
                    if (RequiresSeparator(t1.kind, t2.kind))
                    {
                        foreach (var s in GetSeparators())
                        {
                            yield return (t1.kind, t1.text, s.kind, s.text, t2.kind, t2.text);
                        }
                    }
                }
        }

        private static bool RequiresSeparator(SyntaxKind kind1, SyntaxKind kind2)
        {
            var t1isKeyword = kind1.ToString().EndsWith("Keyword");
            var t2isKeyword = kind2.ToString().EndsWith("Keyword");

            if (kind1 == SyntaxKind.IdentifierToken && kind2 == SyntaxKind.IdentifierToken)
                return true;

            if (t1isKeyword && t2isKeyword)
                return true;

            if (t1isKeyword && kind2 == SyntaxKind.IdentifierToken)
                return true;

            if (kind1 == SyntaxKind.IdentifierToken && t2isKeyword)
                return true;

            if (kind1 == SyntaxKind.NumberToken && kind2 == SyntaxKind.NumberToken)
                return true;

            if (kind1 == SyntaxKind.BangToken && kind2 == SyntaxKind.EqualsToken)
                return true;

            if (kind1 == SyntaxKind.BangToken && kind2 == SyntaxKind.EqualsEqualsToken)
                return true;

            if (kind1 == SyntaxKind.EqualsToken && kind2 == SyntaxKind.EqualsToken)
                return true;

            if (kind1 == SyntaxKind.EqualsToken && kind2 == SyntaxKind.EqualsEqualsToken)
                return true;

            if (kind1 == SyntaxKind.LessToken && kind2 == SyntaxKind.EqualsToken)
                return true;

            if (kind1 == SyntaxKind.GreaterToken && kind2 == SyntaxKind.EqualsToken)
                return true;

            if (kind1 == SyntaxKind.LessToken && kind2 == SyntaxKind.EqualsEqualsToken)
                return true;

            if (kind1 == SyntaxKind.GreaterToken && kind2 == SyntaxKind.EqualsEqualsToken)
                return true;

            return false;
        }
    }
}
