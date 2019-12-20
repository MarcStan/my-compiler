using System;

namespace CodeAnalysis.Binding
{
    internal class BoundBinaryExpression : BoundExpression
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

        public override Type Type => Left.Type;

        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
    }
}
