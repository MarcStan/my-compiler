namespace CodeAnalysis.Binding.Nodes
{
    internal sealed class BoundWhileStatement : BoundStatement
    {
        public BoundWhileStatement(
            BoundExpression condition,
            BoundStatement body)
        {
            Condition = condition;
            Body = body;
        }

        public BoundExpression Condition { get; }
        public BoundStatement Body { get; }

        public override BoundNodeKind Kind => BoundNodeKind.WhileStatement;
    }
}
