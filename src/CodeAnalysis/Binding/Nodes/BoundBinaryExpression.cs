using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding.Nodes
{
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator op, BoundExpression right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public BoundExpression Left { get; }

        public BoundBinaryOperator Operator { get; }

        public BoundExpression Right { get; }

        public override TypeSymbol Type => Operator.Type;

        public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;
    }
}
