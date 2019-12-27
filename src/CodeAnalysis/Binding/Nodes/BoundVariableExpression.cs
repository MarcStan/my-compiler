using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding.Nodes
{
    internal sealed class BoundVariableExpression : BoundExpression
    {
        public BoundVariableExpression(VariableSymbol variable)
        {
            Variable = variable;
        }

        public override BoundNodeKind Kind => BoundNodeKind.VariableExpression;
        public VariableSymbol Variable { get; }

        public override TypeSymbol Type => Variable.Type;
    }
}
