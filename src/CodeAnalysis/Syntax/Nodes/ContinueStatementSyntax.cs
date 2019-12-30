using CodeAnalysis.Syntax.Nodes;

namespace CodeAnalysis.Syntax
{
    internal sealed class ContinueStatementSyntax : StatementSyntax
    {
        public ContinueStatementSyntax(SyntaxToken keyword)
        {
            Keyword = keyword;
        }

        public SyntaxToken Keyword { get; }

        public override SyntaxKind Kind => SyntaxKind.ContinueStatement;
    }
}
