namespace CodeAnalysis.Binding.Nodes
{
    internal sealed class BoundLabelStatement : BoundStatement
    {
        public BoundLabelStatement(BoundLabel label)
        {
            Label = label;
        }

        public BoundLabel Label { get; }

        public override BoundNodeKind Kind => BoundNodeKind.LabelStatement;
    }
}
