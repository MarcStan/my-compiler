namespace CodeAnalysis.Syntax.Nodes
{
    public sealed class TypeClauseSyntax : SyntaxNode
    {
        public TypeClauseSyntax(
            SyntaxToken colon,
            SyntaxToken identifier)
        {
            Colon = colon;
            Identifier = identifier;
        }

        public SyntaxToken Colon { get; }
        public SyntaxToken Identifier { get; }
        public override SyntaxKind Kind => SyntaxKind.TypeClause;
    }
}
