using System;
using System.Collections.Generic;

namespace CodeAnalysis.Syntax
{
    public static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.BangToken:
                case SyntaxKind.TildeToken:
                    return 6;
                default:
                    return 0;
            }
        }

        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.StarToken:
                case SyntaxKind.SlashToken:
                    return 5;

                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 4;

                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.BangEqualsToken:
                case SyntaxKind.LessToken:
                case SyntaxKind.LessOrEqualsToken:
                case SyntaxKind.GreaterToken:
                case SyntaxKind.GreaterOrEqualsToken:
                    return 3;

                case SyntaxKind.AmpersandToken:
                case SyntaxKind.AmpersandAmpersandToken:
                    return 2;

                case SyntaxKind.PipeToken:
                case SyntaxKind.PipePipeToken:
                case SyntaxKind.HatToken:
                    return 1;
                default:
                    return 0;
            }
        }

        public static SyntaxKind GetKeywordKind(string text)
            => text switch
            {
                "else" => SyntaxKind.ElseKeyword,
                "false" => SyntaxKind.FalseKeyword,
                "for" => SyntaxKind.ForKeyword,
                "if" => SyntaxKind.IfKeyword,
                "let" => SyntaxKind.LetKeyword,
                "true" => SyntaxKind.TrueKeyword,
                "var" => SyntaxKind.VarKeyword,
                "to" => SyntaxKind.ToKeyword,
                "while" => SyntaxKind.WhileKeyword,
                "do" => SyntaxKind.DoKeyword,
                "function" => SyntaxKind.FunctionKeyword,
                _ => SyntaxKind.IdentifierToken,
            };

        public static string GetText(SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.PlusToken => "+",
                SyntaxKind.MinusToken => "-",
                SyntaxKind.StarToken => "*",
                SyntaxKind.SlashToken => "/",
                SyntaxKind.BangToken => "!",
                SyntaxKind.EqualsToken => "=",
                SyntaxKind.AmpersandAmpersandToken => "&&",
                SyntaxKind.PipePipeToken => "||",
                SyntaxKind.LessToken => "<",
                SyntaxKind.LessOrEqualsToken => "<=",
                SyntaxKind.GreaterToken => ">",
                SyntaxKind.GreaterOrEqualsToken => ">=",
                SyntaxKind.AmpersandToken => "&",
                SyntaxKind.PipeToken => "|",
                SyntaxKind.TildeToken => "~",
                SyntaxKind.HatToken => "^",
                SyntaxKind.EqualsEqualsToken => "==",
                SyntaxKind.BangEqualsToken => "!=",
                SyntaxKind.OpenParenthesisToken => "(",
                SyntaxKind.CloseParenthesisToken => ")",
                SyntaxKind.ColonToken => ":",
                SyntaxKind.CommaToken => ",",
                SyntaxKind.OpenBraceToken => "{",
                SyntaxKind.CloseBraceToken => "}",
                SyntaxKind.ElseKeyword => "else",
                SyntaxKind.FalseKeyword => "false",
                SyntaxKind.IfKeyword => "if",
                SyntaxKind.ForKeyword => "for",
                SyntaxKind.FunctionKeyword => "function",
                SyntaxKind.TrueKeyword => "true",
                SyntaxKind.VarKeyword => "var",
                SyntaxKind.ToKeyword => "to",
                SyntaxKind.DoKeyword => "do",
                SyntaxKind.WhileKeyword => "while",
                SyntaxKind.LetKeyword => "let",
                _ => null
            };

        public static IEnumerable<SyntaxKind> GetUnaryOperatorKinds()
        {
            var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            foreach (var kind in kinds)
            {
                if (GetUnaryOperatorPrecedence(kind) > 0)
                    yield return kind;
            }
        }

        public static IEnumerable<SyntaxKind> GetBinaryOperatorKinds()
        {
            var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            foreach (var kind in kinds)
            {
                if (GetBinaryOperatorPrecedence(kind) > 0)
                    yield return kind;
            }
        }
    }
}
