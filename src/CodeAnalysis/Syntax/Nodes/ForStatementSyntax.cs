namespace CodeAnalysis.Syntax.Nodes
{
    public sealed class ForStatementSyntax : StatementSyntax
    {
        public ForStatementSyntax(
            SyntaxToken keyword,
            SyntaxToken identifier,
            SyntaxToken equals,
            ExpressionSyntax lowerBound,
            SyntaxToken to,
            ExpressionSyntax upperBound,
            StatementSyntax body)
        {
            Keyword = keyword;
            Identifier = identifier;
            EqualsToken = equals;
            LowerBound = lowerBound;
            To = to;
            UpperBound = upperBound;
            Body = body;
        }

        public override SyntaxKind Kind => SyntaxKind.ForStatement;

        public SyntaxToken Keyword { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax LowerBound { get; }
        public SyntaxToken To { get; }
        public ExpressionSyntax UpperBound { get; }
        public StatementSyntax Body { get; }
    }
}
