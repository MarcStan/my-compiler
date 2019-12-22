using CodeAnalysis.Syntax;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeAnalysis.Tests
{

    internal sealed class AssertingEnumerator : IDisposable
    {
        private readonly IEnumerator<SyntaxNode> _enumerator;
        private bool _hasError;

        public AssertingEnumerator(SyntaxNode node)
        {
            _enumerator = Flatten(node).GetEnumerator();
        }

        private static IEnumerable<SyntaxNode> Flatten(SyntaxNode node)
        {
            var stack = new Stack<SyntaxNode>();
            stack.Push(node);
            while (stack.Any())
            {
                var n = stack.Pop();
                yield return n;

                foreach (var c in n.Children.Reverse())
                    stack.Push(c);
            }
        }

        public void AssertToken(SyntaxKind kind, string text)
        {
            try
            {
                _enumerator.MoveNext().Should().BeTrue();

                var token = _enumerator.Current;
                token.Should().BeOfType<SyntaxToken>();

                token.Kind.Should().Be(kind);
                var st = (SyntaxToken)token;
                st.Text.Should().Be(text);
            }
            catch
            {
                _hasError = true;
                throw;
            }
        }

        public void AssertNode(SyntaxKind kind)
        {
            try
            {
                _enumerator.MoveNext().Should().BeTrue();

                var token = _enumerator.Current;
                token.Kind.Should().Be(kind);
                token.Should().NotBeOfType<SyntaxToken>();
            }
            catch
            {
                _hasError = true;
                throw;
            }
        }

        public void Dispose()
        {
            if (!_hasError)
                _enumerator.MoveNext().Should().BeFalse();

            _enumerator.Dispose();
        }
    }
}
