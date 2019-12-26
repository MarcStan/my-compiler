namespace CodeAnalysis.Binding.Nodes
{
    internal sealed class BoundIfStatement : BoundStatement
    {
        public BoundIfStatement(
            BoundExpression condition,
            BoundStatement thenStatement,
            BoundStatement elseStatement)
        {
            Condition = condition;
            ThenStatement = thenStatement;
            ElseStatement = elseStatement;
        }

        public BoundExpression Condition { get; }
        public BoundStatement ThenStatement { get; }
        public BoundStatement ElseStatement { get; }

        public override BoundNodeKind Kind => BoundNodeKind.IfStatement;
    }
}
