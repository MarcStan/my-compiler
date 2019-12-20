using CodeAnalysis.Nodes;
using System.Collections.Generic;

namespace CodeAnalysis
{
    internal class Parser
    {
        private readonly SyntaxToken[] _tokens;
        private int _position;
        private readonly List<string> _diagnostics = new List<string>();

        public Parser(string text)
        {
            var lexer = new Lexer(text);
            SyntaxToken token;

            var tokens = new List<SyntaxToken>();
            do
            {
                token = lexer.GetNextToken();
                if (token.Kind != SyntaxKind.Whitespace &&
                    token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndOfFileToken);

            _tokens = tokens.ToArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _tokens.Length)
                return _tokens[^1];
            return _tokens[index];
        }

        public SyntaxToken Current
            => Peek(0);

        public IReadOnlyList<string> Diagnostics => _diagnostics;

        private SyntaxToken GetNextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        private SyntaxToken MatchToken(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return GetNextToken();

            _diagnostics.Add($"Error: Unexpected token <{Current.Kind}>, expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        public SyntaxTree Parse()
        {
            var expression = ParseExpression();
            var eof = MatchToken(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(expression, eof, _diagnostics);
        }

        private ExpressionSyntax ParseExpression(int parentPrecedence = 0)
        {
            var left = ParsePrimaryExpression();
            while (true)
            {
                var precedence = GetBinaryOperatorPrecedence(Current.Kind);
                if (precedence == 0 || precedence <= parentPrecedence)
                    break;

                var operatorToken = GetNextToken();
                var right = ParseExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }
            return left;
        }

        private static int GetBinaryOperatorPrecedence(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.StarToken:
                case SyntaxKind.SlashToken:
                    return 2;
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 1;
                default:
                    return 0;
            }
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            if (Current.Kind == SyntaxKind.OpenParenthesisToken)
            {
                var left = GetNextToken();
                var expression = ParseExpression();
                var right = MatchToken(SyntaxKind.CloseParenthesisToken);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }
            var numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }
    }
}
