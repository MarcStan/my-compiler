namespace CodeAnalysis.Syntax.Nodes
{
    public sealed class GlobalStatementSyntax : MemberSyntax
    {
        public GlobalStatementSyntax(StatementSyntax statement)
        {
            Statement = statement;
        }

        public StatementSyntax Statement { get; }
        public override SyntaxKind Kind => SyntaxKind.GlobalStatement;
    }
}
