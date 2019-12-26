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
        GoToStatement,
        LabelStatement,

        VariableDeclaration,

        // expressions
        UnaryExpression,
        BinaryExpression,
        LiteralExpression,
        VariableExpression,
        AssignmentExpression,
        ConditionalGoToStatement,
    }
}
