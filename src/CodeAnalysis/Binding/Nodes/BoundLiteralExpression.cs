using CodeAnalysis.Symbols;
using System;

namespace CodeAnalysis.Binding.Nodes
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value)
        {
            Value = value;

            if (value is bool)
                Type = TypeSymbol.Bool;
            else if (value is int)
                Type = TypeSymbol.Int;
            else if (value is string)
                Type = TypeSymbol.String;
            else
                throw new ArgumentException($"Unexpected literal '{value}' of type {value.GetType()}");
        }

        public object Value { get; }

        public override TypeSymbol Type { get; }

        public static explicit operator BoundLiteralExpression(BoundStatement v)
        {
            throw new NotImplementedException();
        }

        public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
    }
}
