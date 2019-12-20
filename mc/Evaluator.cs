using mc.Nodes;
using System;

namespace mc
{
    public class Evaluator
    {
        private readonly ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            _root = root;
        }

        public int Evaluate()
            => EvaluateExpression(_root);

        private int EvaluateExpression(ExpressionSyntax expr)
        {
            if (expr is NumberExpressionSyntax n)
                return (int)n.NumberToken.Value;
            if (expr is BinaryExpressionSyntax b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                switch (b.OperatorToken.Kind)
                {
                    case SyntaxKind.PlusToken:
                        return left + right;
                    case SyntaxKind.MinusToken:
                        return left - right;
                    case SyntaxKind.StarToken:
                        return left * right;
                    case SyntaxKind.SlashToken:
                        return left / right;
                    default:
                        throw new ArgumentException($"Unexpected binary operator {b.OperatorToken.Kind}");
                }
            }

            throw new ArgumentException($"Unexpected nod {expr.Kind}");
        }
    }
}
