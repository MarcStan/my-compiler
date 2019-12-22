using System.Collections.Generic;

namespace CodeAnalysis.Syntax.Nodes
{
    public sealed class NameExpressionSyntax : ExpressionSyntax
    {
        public NameExpressionSyntax(SyntaxToken identifierToken)
        {
            IdentifierToken = identifierToken;
        }

        public override SyntaxKind Kind => SyntaxKind.NameExpression;
        public SyntaxToken IdentifierToken { get; }

        public override IEnumerable<SyntaxNode> Children
        {
            get
            {
                yield return IdentifierToken;
            }
        }
    }
}
