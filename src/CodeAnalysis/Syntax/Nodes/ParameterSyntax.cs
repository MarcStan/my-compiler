namespace CodeAnalysis.Syntax.Nodes
{
    public sealed class ParameterSyntax : SyntaxNode
    {
        public ParameterSyntax(
            SyntaxToken identifier,
            TypeClauseSyntax typeClause)
        {
            Identifier = identifier;
            TypeClause = typeClause;
        }

        public override SyntaxKind Kind => SyntaxKind.Parameter;

        public SyntaxToken Identifier { get; }
        public TypeClauseSyntax TypeClause { get; }
    }
}
