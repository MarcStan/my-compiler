﻿namespace CodeAnalysis.Syntax
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
        ElseKeyword,
        LetKeyword,
        IfKeyword,
        VarKeyword,
        FalseKeyword,
        TrueKeyword,
        IdentifierToken,
        WhileKeyword,

        // nodes
        CompilationUnit,
        ElseClause,

        // statements
        BlockStatement,
        ExpressionStatement,
        IfStatement,
        WhileStatement,
        VariableDeclaration,
    }
}
