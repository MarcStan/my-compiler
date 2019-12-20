namespace CodeAnalysis
{
    public enum SyntaxKind
    {
        Unknown = 0,
        NumberToken,
        Whitespace,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        BadToken,
        EndOfFileToken,
        NumberExpression,
        BinaryExpression,
        ParenthesizedExpression
    }
}
