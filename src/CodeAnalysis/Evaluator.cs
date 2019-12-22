using CodeAnalysis.Binding;
using System;
using System.Collections.Generic;

namespace CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly BoundExpression _root;
        private readonly Dictionary<VariableSymbol, object> _variables;

        public Evaluator(BoundExpression root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
            => EvaluateExpression(_root);

        private object EvaluateExpression(BoundExpression expr)
            => expr.Kind switch
            {
                BoundNodeKind.UnaryExpression => EvaluateUnaryExpression((BoundUnaryExpression)expr),
                BoundNodeKind.BinaryExpression => EvaluateBinaryExpression((BoundBinaryExpression)expr),
                BoundNodeKind.LiteralExpression => EvaluateLiteralExpression((BoundLiteralExpression)expr),
                BoundNodeKind.VariableExpression => EvaluateVariableExpression((BoundVariableExpression)expr),
                BoundNodeKind.AssignmentExpression => EvaluateAssignmentExpression((BoundAssignmentExpression)expr),
                _ => throw new ArgumentException($"Unexpected node {expr.Kind}"),
            };

        private object EvaluateUnaryExpression(BoundUnaryExpression u)
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

        private object EvaluateBinaryExpression(BoundBinaryExpression b)
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

        private object EvaluateAssignmentExpression(BoundAssignmentExpression a)
        {
            var value = EvaluateExpression(a.Expression);
            _variables[a.Variable] = value;
            return value;
        }

        private object EvaluateVariableExpression(BoundVariableExpression v)
            => _variables[v.Variable];

        private object EvaluateLiteralExpression(BoundLiteralExpression n)
            => n.Value;
    }
}
