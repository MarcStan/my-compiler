namespace CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        Unknown = 0,

        // statements
        BlockStatement,
        ExpressionStatement,
        ForStatement,
        IfStatement,
        WhileStatement,
        DoWhileStatement,
        GoToStatement,
        LabelStatement,
        ConditionalGoToStatement,

        VariableDeclaration,

        // expressions
        UnaryExpression,
        BinaryExpression,
        LiteralExpression,
        VariableExpression,
        AssignmentExpression,
        ErrorExpression,
        CallExpression,
        ConversionExpression,
    }
}
