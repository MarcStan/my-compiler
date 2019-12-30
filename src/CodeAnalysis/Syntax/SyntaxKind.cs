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
        // bitwise operator tokens
        PipeToken,
        AmpersandToken,
        TildeToken,
        HatToken,
        StringToken,
        CommaToken,
        ColonToken,

        // expressions
        LiteralExpression,
        NameExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        AssignmentExpression,
        CallExpression,

        // keyword
        ElseKeyword,
        LetKeyword,
        IfKeyword,
        VarKeyword,
        FalseKeyword,
        TrueKeyword,
        IdentifierToken,
        DoKeyword,
        WhileKeyword,
        ForKeyword,
        ToKeyword,
        FunctionKeyword,
        BreakKeyword,
        ContinueKeyword,

        // nodes
        CompilationUnit,
        ElseClause,
        TypeClause,
        Parameter,

        // statements
        BlockStatement,
        ExpressionStatement,
        IfStatement,
        WhileStatement,
        ForStatement,
        DoWhileStatement,
        GlobalStatement,
        BreakStatement,
        ContinueStatement,

        VariableDeclaration,
        FunctionDeclaration,
    }
}
