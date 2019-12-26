namespace CodeAnalysis.Binding.Nodes
{
    internal sealed class BoundGoToStatement : BoundStatement
    {
        public BoundGoToStatement(LabelSymbol label)
        {
            Label = label;
        }

        public override BoundNodeKind Kind => BoundNodeKind.GoToStatement;

        public LabelSymbol Label { get; }
    }
}
