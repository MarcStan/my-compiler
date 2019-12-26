using System;

namespace CodeAnalysis.Binding.Nodes
{
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public BoundAssignmentExpression(VariableSymbol variable, BoundExpression expression)
        {
            Variable = variable;
            Expression = expression;
        }

        public override Type Type => Expression.Type;

        public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;

        public VariableSymbol Variable { get; }
        public BoundExpression Expression { get; }
    }
}
