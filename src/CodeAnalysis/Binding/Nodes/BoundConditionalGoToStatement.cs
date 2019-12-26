namespace CodeAnalysis.Binding.Nodes
{
    internal sealed class BoundConditionalGoToStatement : BoundStatement
    {
        public BoundConditionalGoToStatement(LabelSymbol label, BoundExpression condition, bool jumpIfFalse = false)
        {
            Label = label;
            Condition = condition;
            JumpIfFalse = jumpIfFalse;
        }

        public override BoundNodeKind Kind => BoundNodeKind.ConditionalGoToStatement;

        public LabelSymbol Label { get; }
        public BoundExpression Condition { get; }
        public bool JumpIfFalse { get; }
    }
}
