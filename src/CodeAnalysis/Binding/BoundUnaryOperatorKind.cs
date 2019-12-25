namespace CodeAnalysis.Binding
{
    internal enum BoundUnaryOperatorKind
    {
        Unknown = 0,
        Identity,
        Negation,
        LogicalNegation,
        OnesComplement
    }
}
