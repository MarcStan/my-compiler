using System;

namespace CodeAnalysis.Binding
{
    internal class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperator op, BoundExpression operand)
        {
            Operator = op;
            Operand = operand;
        }

        public override Type Type => Operand.Type;

        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;

        public BoundUnaryOperator Operator { get; }

        public BoundExpression Operand { get; }
    }
}
