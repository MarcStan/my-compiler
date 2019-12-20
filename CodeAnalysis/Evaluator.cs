using CodeAnalysis.Binding;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Repl")]
namespace CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly BoundExpression _root;

        public Evaluator(BoundExpression root)
        {
            _root = root;
        }

        public int Evaluate()
            => EvaluateExpression(_root);

        private int EvaluateExpression(BoundExpression expr)
        {
            if (expr is BoundLiteralExpression n)
                return (int)n.Value;

            if (expr is BoundUnaryExpression u)
            {
                var operand = EvaluateExpression(u.Operand);
                switch (u.OperatorKind)
                {
                    case BoundUnaryOperatorKind.Identity:
                        return operand;
                    case BoundUnaryOperatorKind.Negation:
                        return -operand;
                    default:
                        throw new ArgumentException($"Unexpected unary operator {u.OperatorKind}");
                }
            }

            if (expr is BoundBinaryExpression b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                switch (b.OperatorKind)
                {
                    case BoundBinaryOperatorKind.Addition:
                        return left + right;
                    case BoundBinaryOperatorKind.Subtraction:
                        return left - right;
                    case BoundBinaryOperatorKind.Multiplication:
                        return left * right;
                    case BoundBinaryOperatorKind.Division:
                        return left / right;
                    default:
                        throw new ArgumentException($"Unexpected binary operator {b.OperatorKind}");
                }
            }

            throw new ArgumentException($"Unexpected nod {expr.Kind}");
        }
    }
}
