using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding.Nodes
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperator op, BoundExpression operand)
        {
            Operator = op;
            Operand = operand;
        }

        public override TypeSymbol Type => Operator.Type;

        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;

        public BoundUnaryOperator Operator { get; }

        public BoundExpression Operand { get; }
    }
}
