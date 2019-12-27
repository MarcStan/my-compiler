using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding.Nodes
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract TypeSymbol Type { get; }
    }
}
