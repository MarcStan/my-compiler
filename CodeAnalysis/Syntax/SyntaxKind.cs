namespace CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        Unknown = 0,

        // tokens
        BadToken,
        EndOfFileToken,
        Whitespace,
        NumberToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        BangToken,
        AmpersandToken,
        PipeToken,

        // expressions
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,

        // keyword
        FalseKeyword,
        TrueKeyword,
        IdentifierToken
    }
}
