using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Nodes;
using CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;

namespace CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly BoundBlockStatement _root;
        private readonly Dictionary<VariableSymbol, object> _variables;

        private object _lastValue;
        private Random _random;

        public Evaluator(BoundBlockStatement root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
        {
            var labelToIndex = new Dictionary<BoundLabel, int>();

            for (int i = 0; i < _root.Statements.Length; i++)
            {
                if (_root.Statements[i] is BoundLabelStatement l)
                    labelToIndex.Add(l.Label, i);
            }

            int index = 0;
            while (index < _root.Statements.Length)
            {
                var s = _root.Statements[index++];
                switch (s.Kind)
                {
                    case BoundNodeKind.ExpressionStatement:
                        EvaluateExpressionStatement((BoundExpressionStatement)s);
                        break;
                    case BoundNodeKind.VariableDeclaration:
                        EvaluateVariableDeclaration((BoundVariableDeclaration)s);
                        break;
                    case BoundNodeKind.ConditionalGoToStatement:
                        var @goto = (BoundConditionalGoToStatement)s;
                        var condition = (bool)EvaluateExpression(@goto.Condition);
                        if ((condition && !@goto.JumpIfFalse) ||
                            (!condition && @goto.JumpIfFalse))
                        {
                            index = labelToIndex[@goto.Label];
                        }
                        break;
                    case BoundNodeKind.GoToStatement:
                        index = labelToIndex[((BoundGoToStatement)s).Label];
                        break;
                    case BoundNodeKind.LabelStatement:
                        break;
                    default:
                        throw new ArgumentException($"Unexpected node {s.Kind}");
                }
            }
            return _lastValue;
        }

        private void EvaluateVariableDeclaration(BoundVariableDeclaration node)
        {
            var value = EvaluateExpression(node.Initializer);
            _variables[node.Variable] = value;
            _lastValue = value;
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
                BoundNodeKind.CallExpression => EvaluateCallExpression((BoundCallExpression)expr),
                BoundNodeKind.ConversionExpression => EvaluateConversionExpression((BoundConversionExpression)expr),
                _ => throw new ArgumentException($"Unexpected node {expr.Kind}"),
            };

        private object EvaluateConversionExpression(BoundConversionExpression node)
        {
            var value = EvaluateExpression(node.Expression);
            if (node.Type == TypeSymbol.Bool)
                return Convert.ToBoolean(value);
            if (node.Type == TypeSymbol.Int)
                return Convert.ToInt32(value);
            if (node.Type == TypeSymbol.String)
                return Convert.ToString(value);

            throw new Exception($"Unexpected type {node.Type}");
        }

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

        private object EvaluateCallExpression(BoundCallExpression node)
        {
            if (node.Function == BuiltinFunctions.Input)
            {
                return Console.ReadLine();
            }
            else if (node.Function == BuiltinFunctions.Print)
            {
                var message = (string)EvaluateExpression(node.Arguments[0]);
                Console.WriteLine(message);
                return null;
            }
            else if (node.Function == BuiltinFunctions.Random)
            {
                var min = (int)EvaluateExpression(node.Arguments[0]);
                var max = (int)EvaluateExpression(node.Arguments[1]);
                if (_random == null)
                    _random = new Random();

                return _random.Next(min, max);
            }
            else
            {
                throw new Exception($"Undefined function {node.Function.Name}!");
            }
        }

        private object EvaluateBinaryExpression(BoundBinaryExpression b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);

            return b.Operator.Kind switch
            {
                BoundBinaryOperatorKind.Addition => b.Type == TypeSymbol.Int ?
                        (object)((int)left + (int)right) :
                        (string)left + (string)right,
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
                BoundBinaryOperatorKind.BitwiseAnd => b.Type == TypeSymbol.Int ?
                        (object)((int)left & (int)right) :
                        (bool)left & (bool)right,
                BoundBinaryOperatorKind.BitwiseOr => b.Type == TypeSymbol.Int ?
                        (object)((int)left | (int)right) :
                        (bool)left | (bool)right,
                BoundBinaryOperatorKind.BitwiseXor => b.Type == TypeSymbol.Int ?
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
