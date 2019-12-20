using System.Collections.Generic;

namespace CodeAnalysis.Syntax
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
            => Peek(0);

        private char LookAhead
            => Peek(1);

        private char Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _text.Length)
                return '\0';
            return _text[index];
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

            if (char.IsLetter(Current))
            {
                var start = _position;

                while (char.IsLetter(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);

                var kind = SyntaxFacts.GetKeywordKind(text);

                return new SyntaxToken(kind, start, text, null);
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

            switch (Current)
            {
                case '+':
                    return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
                case '-':
                    return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
                case '*':
                    return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
                case '/':
                    return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
                case '(':
                    return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
                case ')':
                    return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);
                case '&':
                    if (LookAhead == '&')
                        return new SyntaxToken(SyntaxKind.AmpersandToken, _position += 2, "&&", null);
                    break;
                case '|':
                    if (LookAhead == '|')
                        return new SyntaxToken(SyntaxKind.PipeToken, _position += 2, "&&", null);
                    break;
                case '=':
                    if (LookAhead == '=')
                        return new SyntaxToken(SyntaxKind.EqualsEqualsToken, _position += 2, "==", null);
                    break;
                case '!':
                    if (LookAhead == '=')
                        return new SyntaxToken(SyntaxKind.BangEqualsToken, _position += 2, "!=", null);
                    else
                        return new SyntaxToken(SyntaxKind.BangToken, _position++, "!", null);
            }

            _diagnostics.Add($"Error: bad character input: '{Current}'");
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }
}
