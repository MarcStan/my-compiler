using System.Collections.Generic;

namespace CodeAnalysis.Syntax.Nodes
{
    public sealed class ExpressionStatementSyntax : StatementSyntax
    {
        public ExpressionStatementSyntax(ExpressionSyntax expression)
        {
            Expression = expression;
        }

        public ExpressionSyntax Expression { get; }

        public override SyntaxKind Kind => SyntaxKind.ExpressionStatement;

        public override IEnumerable<SyntaxNode> Children
        {
            get
            {
                yield return Expression;
            }
        }
    }
}
