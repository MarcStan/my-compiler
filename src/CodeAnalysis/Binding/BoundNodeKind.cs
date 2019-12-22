namespace CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        Unknown = 0,

        // statements
        BlockStatement,
        VariableDeclaration,
        ExpressionStatement,

        // expressions
        UnaryExpression,
        BinaryExpression,
        LiteralExpression,
        VariableExpression,
        AssignmentExpression
    }
}
