using System.Collections.Generic;

namespace CodeAnalysis.Syntax.Nodes
{
    public class UnaryExpressionSyntax : ExpressionSyntax
    {
        public UnaryExpressionSyntax(SyntaxToken operatorToken, ExpressionSyntax operand)
        {
            OperatorToken = operatorToken;
            Operand = operand;
        }

        public SyntaxToken OperatorToken { get; }

        public ExpressionSyntax Operand { get; }

        public override SyntaxKind Kind => SyntaxKind.UnaryExpression;

        public override IEnumerable<SyntaxNode> Children
        {
            get
            {
                yield return OperatorToken;
                yield return Operand;
            }
        }
    }
}
