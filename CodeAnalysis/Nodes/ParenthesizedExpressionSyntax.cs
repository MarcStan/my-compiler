using System.Collections.Generic;

namespace CodeAnalysis.Nodes
{
    public class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public ParenthesizedExpressionSyntax(SyntaxToken openParenthesis, ExpressionSyntax expression, SyntaxToken closeParenthesis)
        {
            OpenParenthesis = openParenthesis;
            CloseParenthesis = closeParenthesis;
            Expression = expression;
        }

        public SyntaxToken OpenParenthesis { get; }

        public ExpressionSyntax Expression { get; }

        public SyntaxToken CloseParenthesis { get; }

        public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

        public override IEnumerable<SyntaxNode> Children
        {
            get
            {
                yield return OpenParenthesis;
                yield return Expression;
                yield return CloseParenthesis;
            }
        }
    }
}
