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
        OpenBraceToken,
        CloseBraceToken,
        BangToken,
        AmpersandAmpersandToken,
        PipePipeToken,
        EqualsToken,
        EqualsEqualsToken,
        BangEqualsToken,
        LessToken,
        LessOrEqualsToken,
        GreaterToken,
        GreaterOrEqualsToken,

        // expressions
        LiteralExpression,
        NameExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        AssignmentExpression,

        // keyword
        LetKeyword,
        VarKeyword,
        FalseKeyword,
        TrueKeyword,
        IdentifierToken,

        // nodes
        CompilationUnit,

        // statements
        BlockStatement,
        ExpressionStatement,

        VariableDeclaration,
    }
}
