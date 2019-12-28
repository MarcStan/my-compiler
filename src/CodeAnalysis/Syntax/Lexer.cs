using CodeAnalysis.Symbols;
using CodeAnalysis.Text;
using System.Text;

namespace CodeAnalysis.Syntax
{
    internal class Lexer
    {
        private readonly SourceText _text;
        private int _position;
        private int _start;
        private SyntaxKind _kind;
        private object _value;

        public Lexer(SourceText text)
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

        public DiagnosticsCollection Diagnostics { get; } = new DiagnosticsCollection();

        public SyntaxToken Lex()
        {
            _start = _position;
            _kind = SyntaxKind.BadToken;
            _value = null;

            switch (Current)
            {
                case '\0':
                    _kind = SyntaxKind.EndOfFileToken;
                    break;
                case '+':
                    _kind = SyntaxKind.PlusToken;
                    _position++;
                    break;
                case '-':
                    _kind = SyntaxKind.MinusToken;
                    _position++;
                    break;
                case '*':
                    _kind = SyntaxKind.StarToken;
                    _position++;
                    break;
                case '/':
                    _kind = SyntaxKind.SlashToken;
                    _position++;
                    break;
                case '(':
                    _kind = SyntaxKind.OpenParenthesisToken;
                    _position++;
                    break;
                case ')':
                    _kind = SyntaxKind.CloseParenthesisToken;
                    _position++;
                    break;
                case ',':
                    _kind = SyntaxKind.CommaToken;
                    _position++;
                    break;
                case '{':
                    _kind = SyntaxKind.OpenBraceToken;
                    _position++;
                    break;
                case '}':
                    _kind = SyntaxKind.CloseBraceToken;
                    _position++;
                    break;
                case '&':
                    _position++;
                    if (Current == '&')
                    {
                        _position++;
                        _kind = SyntaxKind.AmpersandAmpersandToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.AmpersandToken;
                    }
                    break;
                case '|':
                    _position++;
                    if (Current == '|')
                    {
                        _position++;
                        _kind = SyntaxKind.PipePipeToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.PipeToken;
                    }
                    break;
                case '^':
                    _kind = SyntaxKind.HatToken;
                    _position++;
                    break;
                case '~':
                    _kind = SyntaxKind.TildeToken;
                    _position++;
                    break;
                case '=':
                    _position++;
                    if (Current == '=')
                    {
                        _kind = SyntaxKind.EqualsEqualsToken;
                        _position++;
                    }
                    else
                    {
                        _kind = SyntaxKind.EqualsToken;
                    }
                    break;
                case '!':
                    _position++;
                    if (Current == '=')
                    {
                        _position++;
                        _kind = SyntaxKind.BangEqualsToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.BangToken;
                    }
                    break;
                case '<':
                    _position++;
                    if (Current == '=')
                    {
                        _position++;
                        _kind = SyntaxKind.LessOrEqualsToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.LessToken;
                    }
                    break;
                case '>':
                    _position++;
                    if (Current == '=')
                    {
                        _position++;
                        _kind = SyntaxKind.GreaterOrEqualsToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.GreaterToken;
                    }
                    break;
                case '"':
                    ReadString();
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    ReadNumberToken();
                    break;
                default:
                    if (char.IsLetter(Current))
                    {
                        ReadIdentififerOrKeyword();
                    }
                    else if (char.IsWhiteSpace(Current))
                    {
                        ReadWhiteSpace();
                    }
                    else
                    {
                        Diagnostics.ReportBadCharacter(_position, Current);
                        _position++;
                    }
                    break;
            }
            var length = _position - _start;
            var text = SyntaxFacts.GetText(_kind) ?? _text.ToString(_start, length);

            return new SyntaxToken(_kind, _start, text, _value);
        }

        private void ReadString()
        {
            _position++;

            var sb = new StringBuilder();
            var done = false;
            while (!done)
            {
                switch (Current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        done = true;
                        Diagnostics.ReportUnterminatedString(new TextSpan(_start, 1));
                        break;
                    case '\\':
                        if (LookAhead == '"')
                        {
                            sb.Append('"');
                            _position++;
                        }
                        else
                        {
                            sb.Append(Current);
                        }
                        _position++;
                        break;
                    case '"':
                        _position++;
                        done = true;
                        break;
                    default:
                        sb.Append(Current);
                        _position++;
                        break;
                }
            }

            _value = sb.ToString();
            _kind = SyntaxKind.StringToken;
        }

        private void ReadIdentififerOrKeyword()
        {
            while (char.IsLetter(Current))
                _position++;

            var length = _position - _start;
            var text = _text.ToString(_start, length);

            _kind = SyntaxFacts.GetKeywordKind(text);
        }

        private void ReadWhiteSpace()
        {
            while (char.IsWhiteSpace(Current))
                _position++;

            _kind = SyntaxKind.Whitespace;
        }

        private void ReadNumberToken()
        {
            while (char.IsDigit(Current))
                _position++;

            var length = _position - _start;
            var text = _text.ToString(_start, length);

            if (!int.TryParse(text, out int value))
                Diagnostics.ReportInvalidNumber(new TextSpan(_start, length), text, TypeSymbol.Int);

            _value = value;
            _kind = SyntaxKind.NumberToken;
        }
    }
}
