using CodeAnalysis.Syntax;
using System.Collections.Generic;

namespace CodeAnalysis.Nodes.Syntax
{
    public class LiteralExpressionSyntax : ExpressionSyntax
    {
        public LiteralExpressionSyntax(SyntaxToken literalToken, object value)
        {
            LiteralToken = literalToken;
            Value = value;
        }

        public LiteralExpressionSyntax(SyntaxToken literalToken)
            : this(literalToken, literalToken.Value)
        {
        }

        public override SyntaxKind Kind => SyntaxKind.LiteralExpression;

        public SyntaxToken LiteralToken { get; }

        public override IEnumerable<SyntaxNode> Children
        {
            get { yield return LiteralToken; }
        }

        public object Value { get; }
    }
}
