using System.Collections.Generic;

namespace CodeAnalysis.Nodes
{
    public class NumberExpressionSyntax : ExpressionSyntax
    {
        public NumberExpressionSyntax(SyntaxToken numberToken)
        {
            NumberToken = numberToken;
        }

        public override SyntaxKind Kind => SyntaxKind.NumberExpression;

        public SyntaxToken NumberToken { get; }

        public override IEnumerable<SyntaxNode> Children
        {
            get { yield return NumberToken; }
        }
    }
}
