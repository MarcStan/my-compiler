using System;

namespace CodeAnalysis.Binding.Nodes
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value)
        {
            Value = value;
        }

        public object Value { get; }

        public override Type Type => Value.GetType();

        public static explicit operator BoundLiteralExpression(BoundStatement v)
        {
            throw new NotImplementedException();
        }

        public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
    }
}
