namespace CodeAnalysis.Binding.Nodes
{
    public enum BoundBinaryOperatorKind
    {
        Unknown = 0,

        Addition,
        Subtraction,
        Multiplication,
        Division,
        LogicalAdd,
        LogicalOr,
        Equals,
        NotEquals,
        Less,
        LessOrEquals,
        Greater,
        GreaterOrEquals,
        BitwiseOr,
        BitwiseAnd,
        BitwiseXor
    }
}
