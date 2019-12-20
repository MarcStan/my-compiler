using CodeAnalysis.Syntax;
using System.Collections.Generic;

namespace CodeAnalysis.Nodes.Syntax
{
    public class LiteralExpressionSyntax : ExpressionSyntax
    {
        public LiteralExpressionSyntax(SyntaxToken literalToken)
        {
            LiteralToken = literalToken;
        }

        public override SyntaxKind Kind => SyntaxKind.LiteralExpression;

        public SyntaxToken LiteralToken { get; }

        public override IEnumerable<SyntaxNode> Children
        {
            get { yield return LiteralToken; }
        }
    }
}
