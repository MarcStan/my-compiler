using System.Collections.Generic;

namespace CodeAnalysis
{
    internal class Lexer
    {
        private readonly List<string> _diagnostics = new List<string>();
        private readonly string _text;
        private int _position;

        public Lexer(string text)
        {
            _text = text;
        }

        private char Current
        {
            get
            {
                if (_position >= _text.Length)
                    return '\0';
                return _text[_position];
            }
        }

        public IReadOnlyList<string> Diagnostics => _diagnostics;

        private void Next()
            => _position++;

        public SyntaxToken GetNextToken()
        {
            if (_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);

            if (char.IsDigit(Current))
            {
                var start = _position;

                while (char.IsDigit(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);

                if (!int.TryParse(text, out int value))
                    _diagnostics.Add($"The number '{text}' isn't a valid Int32");

                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                var start = _position;

                while (char.IsWhiteSpace(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);

                return new SyntaxToken(SyntaxKind.Whitespace, start, text, null);
            }

            if (Current == '+')
                return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
            if (Current == '-')
                return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
            if (Current == '*')
                return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
            if (Current == '/')
                return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
            if (Current == '(')
                return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
            if (Current == ')')
                return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);

            _diagnostics.Add($"Error: bad character input: '{Current}'");
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }
}
