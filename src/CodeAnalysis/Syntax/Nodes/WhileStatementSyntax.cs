namespace CodeAnalysis.Syntax.Nodes
{
    public sealed class WhileStatementSyntax : StatementSyntax
    {
        public WhileStatementSyntax(
            SyntaxToken whileKeyword,
            ExpressionSyntax condition,
            StatementSyntax body)
        {
            WhileKeyword = whileKeyword;
            Condition = condition;
            Body = body;
        }

        public SyntaxToken WhileKeyword { get; }
        public ExpressionSyntax Condition { get; }
        public StatementSyntax Body { get; }

        public override SyntaxKind Kind => SyntaxKind.WhileStatement;
    }
}
