namespace CodeAnalysis.Binding.Nodes
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
