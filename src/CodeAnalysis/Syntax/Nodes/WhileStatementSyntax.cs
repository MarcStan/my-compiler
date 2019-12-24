namespace CodeAnalysis.Syntax.Nodes
{
    public sealed class WhileStatementSyntax : StatementSyntax
    {
        public WhileStatementSyntax(
            SyntaxToken whileKeyword,
            ExpressionSyntax condition,
            StatementSyntax statement)
        {
            WhileKeyword = whileKeyword;
            Condition = condition;
            Statement = statement;
        }

        public SyntaxToken WhileKeyword { get; }
        public ExpressionSyntax Condition { get; }
        public StatementSyntax Statement { get; }

        public override SyntaxKind Kind => SyntaxKind.WhileStatement;
    }
}
