using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Nodes;
using CodeAnalysis.Tests.Helpers;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CodeAnalysis.Tests
{
    public class ParserTests
    {
        [Test]
        [TestCaseSource(nameof(GetBinaryOperatorPairsData))]
        public void BinaryExpression_should_honor_precedence(SyntaxKind op1, SyntaxKind op2)
        {
            var op1Precedence = SyntaxFacts.GetBinaryOperatorPrecedence(op1);
            var op2Precedence = SyntaxFacts.GetBinaryOperatorPrecedence(op2);

            var op1Text = SyntaxFacts.GetText(op1);
            var op2Text = SyntaxFacts.GetText(op2);

            var text = $"a {op1Text} b {op2Text} c";
            var expression = ParseExpression(text);

            if (op1Precedence >= op2Precedence)
            {
                //     op2
                //    /  \
                //   op1  c
                //  /  \
                // a    b

                using (var e = new AssertingEnumerator(expression))
                {
                    e.AssertNode(SyntaxKind.BinaryExpression);
                    e.AssertNode(SyntaxKind.BinaryExpression);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken, "a");
                    e.AssertToken(op1, op1Text);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken, "b");
                    e.AssertToken(op2, op2Text);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken, "c");
                }
            }
            else
            {
                //     op1
                //    /  \
                //   a    op2
                //       /  \
                //      b    c

                using (var e = new AssertingEnumerator(expression))
                {
                    e.AssertNode(SyntaxKind.BinaryExpression);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken, "a");
                    e.AssertToken(op1, op1Text);
                    e.AssertNode(SyntaxKind.BinaryExpression);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken, "b");
                    e.AssertToken(op2, op2Text);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken, "c");
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(GetUnaryOperatorPairsData))]
        public void UnaryExpression_should_honor_precedence(SyntaxKind unary, SyntaxKind binary)
        {
            var unaryPrecedence = SyntaxFacts.GetUnaryOperatorPrecedence(unary);
            var binaryPrecedence = SyntaxFacts.GetBinaryOperatorPrecedence(binary);

            var unaryText = SyntaxFacts.GetText(unary);
            var binaryText = SyntaxFacts.GetText(binary);

            var text = $"{unaryText} a {binaryText} b";
            var expression = ParseExpression(text);

            if (unaryPrecedence >= binaryPrecedence)
            {
                //   binary
                //   /    \
                // unary   b
                //   |
                //   a

                using (var e = new AssertingEnumerator(expression))
                {
                    e.AssertNode(SyntaxKind.BinaryExpression);
                    e.AssertNode(SyntaxKind.UnaryExpression);
                    e.AssertToken(unary, unaryText);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken, "a");
                    e.AssertToken(binary, binaryText);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken, "b");
                }
            }
            else
            {
                //  unary
                //    |
                //  binary
                //  /   \
                // a     b

                using (var e = new AssertingEnumerator(expression))
                {
                    e.AssertNode(SyntaxKind.UnaryExpression);
                    e.AssertToken(unary, unaryText);
                    e.AssertNode(SyntaxKind.BinaryExpression);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken, "a");
                    e.AssertToken(binary, binaryText);
                    e.AssertNode(SyntaxKind.NameExpression);
                    e.AssertToken(SyntaxKind.IdentifierToken, "b");
                }
            }
        }

        private static IEnumerable<object[]> GetBinaryOperatorPairsData()
        {
            foreach (var op1 in SyntaxFacts.GetBinaryOperatorKinds())
                foreach (var op2 in SyntaxFacts.GetBinaryOperatorKinds())
                {
                    yield return new object[] { op1, op2 };
                }
        }

        private static IEnumerable<object[]> GetUnaryOperatorPairsData()
        {
            foreach (var unary in SyntaxFacts.GetUnaryOperatorKinds())
                foreach (var binary in SyntaxFacts.GetBinaryOperatorKinds())
                {
                    yield return new object[] { unary, binary };
                }
        }

        private ExpressionSyntax ParseExpression(string text)
        {
            var statement = SyntaxTree.Parse(text).Root.Members.Single();
            statement.Should().BeOfType<GlobalStatementSyntax>();

            return ((ExpressionStatementSyntax)((GlobalStatementSyntax)statement).Statement).Expression;
        }
    }
}
