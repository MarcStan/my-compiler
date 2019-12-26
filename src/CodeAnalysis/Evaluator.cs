using CodeAnalysis.Binding;
using System;
using System.Collections.Generic;

namespace CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly BoundStatement _root;
        private readonly Dictionary<VariableSymbol, object> _variables;

        private object _lastValue;

        public Evaluator(BoundStatement root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
        {
            EvaluateStatement(_root);
            return _lastValue;
        }

        private void EvaluateStatement(BoundStatement node)
        {
            switch (node.Kind)
            {
                case BoundNodeKind.BlockStatement:
                    EvaluateBlockStatement((BoundBlockStatement)node);
                    break;
                case BoundNodeKind.ExpressionStatement:
                    EvaluateExpressionStatement((BoundExpressionStatement)node);
                    break;
                case BoundNodeKind.VariableDeclaration:
                    EvaluateVariableDeclaration((BoundVariableDeclaration)node);
                    break;
                case BoundNodeKind.IfStatement:
                    EvaluateIfStatement((BoundIfStatement)node);
                    break;
                case BoundNodeKind.WhileStatement:
                    EvaluateWhileStatement((BoundWhileStatement)node);
                    break;
                default:
                    throw new ArgumentException($"Unexpected node {node.Kind}");
            }
        }

        private void EvaluateWhileStatement(BoundWhileStatement node)
        {
            while ((bool)EvaluateExpression(node.Condition))
                EvaluateStatement(node.Body);
        }

        private void EvaluateIfStatement(BoundIfStatement node)
        {
            var condition = (bool)EvaluateExpression(node.Condition);
            if (condition)
                EvaluateStatement(node.ThenStatement);
            else if (node.ElseStatement != null)
                EvaluateStatement(node.ElseStatement);
        }

        private void EvaluateVariableDeclaration(BoundVariableDeclaration node)
        {
            var value = EvaluateExpression(node.Initializer);
            _variables[node.Variable] = value;
            _lastValue = value;
        }

        private void EvaluateBlockStatement(BoundBlockStatement statement)
        {
            foreach (var s in statement.Statements)
                EvaluateStatement(s);
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement node)
        {
            _lastValue = EvaluateExpression(node.Expression);
        }

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
            return u.Operator.Kind switch
            {
                BoundUnaryOperatorKind.Identity => (int)operand,
                BoundUnaryOperatorKind.Negation => -(int)operand,
                BoundUnaryOperatorKind.LogicalNegation => !(bool)operand,
                BoundUnaryOperatorKind.OnesComplement => ~(int)operand,
                _ => throw new ArgumentException($"Unexpected unary operator {u.Operator}"),
            };
        }

        private object EvaluateBinaryExpression(BoundBinaryExpression b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);

            return b.Operator.Kind switch
            {
                BoundBinaryOperatorKind.Addition => (int)left + (int)right,
                BoundBinaryOperatorKind.Subtraction => (int)left - (int)right,
                BoundBinaryOperatorKind.Multiplication => (int)left * (int)right,
                BoundBinaryOperatorKind.Division => (int)left / (int)right,
                BoundBinaryOperatorKind.LogicalAdd => (bool)left && (bool)right,
                BoundBinaryOperatorKind.LogicalOr => (bool)left || (bool)right,
                BoundBinaryOperatorKind.Equals => Equals(left, right),
                BoundBinaryOperatorKind.NotEquals => !Equals(left, right),
                BoundBinaryOperatorKind.Less => (int)left < (int)right,
                BoundBinaryOperatorKind.LessOrEquals => (int)left <= (int)right,
                BoundBinaryOperatorKind.Greater => (int)left > (int)right,
                BoundBinaryOperatorKind.GreaterOrEquals => (int)left >= (int)right,
                BoundBinaryOperatorKind.BitwiseAnd => b.Type == typeof(int) ?
                        (object)((int)left & (int)right) :
                        (bool)left & (bool)right,
                BoundBinaryOperatorKind.BitwiseOr => b.Type == typeof(int) ?
                        (object)((int)left | (int)right) :
                        (bool)left | (bool)right,
                BoundBinaryOperatorKind.BitwiseXor => b.Type == typeof(int) ?
                        (object)((int)left ^ (int)right) :
                        (bool)left ^ (bool)right,
                _ => throw new ArgumentException($"Unexpected binary operator {b.Operator}"),
            };
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
