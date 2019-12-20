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

        public object Evaluate()
            => EvaluateExpression(_root);

        private object EvaluateExpression(BoundExpression expr)
        {
            if (expr is BoundLiteralExpression n)
                return n.Value;

            if (expr is BoundUnaryExpression u)
            {
                var operand = EvaluateExpression(u.Operand);
                switch (u.Operator.Kind)
                {
                    case BoundUnaryOperatorKind.Identity:
                        return (int)operand;
                    case BoundUnaryOperatorKind.Negation:
                        return -(int)operand;
                    case BoundUnaryOperatorKind.LogicalNegation:
                        return !(bool)operand;
                    default:
                        throw new ArgumentException($"Unexpected unary operator {u.Operator}");
                }
            }

            if (expr is BoundBinaryExpression b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                switch (b.Operator.Kind)
                {
                    case BoundBinaryOperatorKind.Addition:
                        return (int)left + (int)right;
                    case BoundBinaryOperatorKind.Subtraction:
                        return (int)left - (int)right;
                    case BoundBinaryOperatorKind.Multiplication:
                        return (int)left * (int)right;
                    case BoundBinaryOperatorKind.Division:
                        return (int)left / (int)right;
                    case BoundBinaryOperatorKind.LogicalAdd:
                        return (bool)left && (bool)right;
                    case BoundBinaryOperatorKind.LogicalOr:
                        return (bool)left || (bool)right;
                    case BoundBinaryOperatorKind.Equals:
                        return Equals(left, right);
                    case BoundBinaryOperatorKind.NotEquals:
                        return !Equals(left, right);
                    default:
                        throw new ArgumentException($"Unexpected binary operator {b.Operator}");
                }
            }

            throw new ArgumentException($"Unexpected nod {expr.Kind}");
        }
    }
}
