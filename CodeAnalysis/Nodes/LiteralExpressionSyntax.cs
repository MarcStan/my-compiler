using System.Collections.Generic;

namespace CodeAnalysis.Nodes
{
    public class LiteralExpressionSyntax : ExpressionSyntax
    {
        public LiteralExpressionSyntax(SyntaxToken literalToken)
        {
            LiteralToken = literalToken;
        }

        public override SyntaxKind Kind => SyntaxKind.NumberExpression;

        public SyntaxToken LiteralToken { get; }

        public override IEnumerable<SyntaxNode> Children
        {
            get { yield return LiteralToken; }
        }
    }
}
