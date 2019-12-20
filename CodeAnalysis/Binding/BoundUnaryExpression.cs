using System;

namespace CodeAnalysis.Binding
{
    internal class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperatorKind operatorKind, BoundExpression operand)
        {
            OperatorKind = operatorKind;
            Operand = operand;
        }

        public override Type Type => Operand.Type;

        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;

        public BoundUnaryOperatorKind OperatorKind { get; }

        public BoundExpression Operand { get; }
    }
}
