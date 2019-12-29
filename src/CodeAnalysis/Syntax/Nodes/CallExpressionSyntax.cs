namespace CodeAnalysis.Syntax.Nodes
{
    public sealed class CallExpressionSyntax : ExpressionSyntax
    {
        public CallExpressionSyntax(
            SyntaxToken identifier,
            SyntaxToken openParenthesis,
            SeparatedSyntaxList<ExpressionSyntax> arguments,
            SyntaxToken closeParenthesis)
        {
            Identifier = identifier;
            OpenParenthesis = openParenthesis;
            Parameters = arguments;
            CloseParenthesis = closeParenthesis;
        }

        public override SyntaxKind Kind => SyntaxKind.CallExpression;

        public SyntaxToken Identifier { get; }
        public SyntaxToken OpenParenthesis { get; }
        public SeparatedSyntaxList<ExpressionSyntax> Parameters { get; }
        public SyntaxToken CloseParenthesis { get; }
    }
}
