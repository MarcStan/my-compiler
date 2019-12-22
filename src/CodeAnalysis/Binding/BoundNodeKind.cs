namespace CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        Unknown = 0,
        UnaryExpression,
        BinaryExpression,
        LiteralExpression,
        VariableExpression,
        AssignmentExpression
    }
}
