using CodeAnalysis.Nodes;
using System;

namespace CodeAnalysis
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
            if (expr is LiteralExpressionSyntax n)
                return (int)n.LiteralToken.Value;

            if (expr is UnaryExpressionSyntax u)
            {
                var operand = EvaluateExpression(u.Operand);
                if (u.OperatorToken.Kind == SyntaxKind.PlusToken)
                    return operand;
                else if (u.OperatorToken.Kind == SyntaxKind.MinusToken)
                    return -operand;

                throw new ArgumentException($"Unexpected unary operator {u.OperatorToken.Kind}");
            }

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

            if (expr is ParenthesizedExpressionSyntax p)
                return EvaluateExpression(p.Expression);

            throw new ArgumentException($"Unexpected nod {expr.Kind}");
        }
    }
}
