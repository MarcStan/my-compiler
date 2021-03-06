namespace CodeAnalysis.Syntax.Nodes
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
    }
}
