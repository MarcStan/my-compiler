using CodeAnalysis.Binding.Nodes;
using System;
using System.Collections.Immutable;

namespace CodeAnalysis.Binding
{
    internal abstract class BoundTreeRewriter
    {
        public virtual BoundExpression RewriteExpression(BoundExpression s)
            => s.Kind switch
            {
                BoundNodeKind.LiteralExpression => RewriteLiteralExpression((BoundLiteralExpression)s),
                BoundNodeKind.VariableExpression => RewriteVariableExpression((BoundVariableExpression)s),
                BoundNodeKind.AssignmentExpression => RewriteAssignmentExpression((BoundAssignmentExpression)s),
                BoundNodeKind.UnaryExpression => RewriteUnaryExpression((BoundUnaryExpression)s),
                BoundNodeKind.BinaryExpression => RewriteBinaryExpression((BoundBinaryExpression)s),
                _ => throw new ArgumentException($"Cannot rewrite {s.Kind}")
            };

        public virtual BoundStatement RewriteStatement(BoundStatement s)
            => s.Kind switch
            {
                BoundNodeKind.BlockStatement => RewriteBlockStatement((BoundBlockStatement)s),
                BoundNodeKind.ExpressionStatement => RewriteExpressionStatement((BoundExpressionStatement)s),
                BoundNodeKind.ForStatement => RewriteForStatement((BoundForStatement)s),
                BoundNodeKind.IfStatement => RewriteIfStatement((BoundIfStatement)s),
                BoundNodeKind.WhileStatement => RewriteWhileStatement((BoundWhileStatement)s),
                BoundNodeKind.VariableDeclaration => RewriteVariableDeclaration((BoundVariableDeclaration)s),
                _ => throw new ArgumentException($"Cannot rewrite {s.Kind}")
            };

        protected virtual BoundStatement RewriteVariableDeclaration(BoundVariableDeclaration node)
        {
            var initializer = RewriteExpression(node.Initializer);
            if (initializer == node.Initializer)
                return node;

            return new BoundVariableDeclaration(node.Variable, initializer);
        }

        protected virtual BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            var condition = RewriteExpression(node.Condition);
            var body = RewriteStatement(node.Body);
            if (condition == node.Condition &&
                body == node.Body)
                return node;

            return new BoundWhileStatement(condition, body);
        }

        protected virtual BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            var condition = RewriteExpression(node.Condition);
            var thenStatement = RewriteStatement(node.ThenStatement);
            var elseStatement = node.ElseStatement == null ? null : RewriteStatement(node.ElseStatement);
            if (condition == node.Condition &&
                thenStatement == node.ThenStatement &&
                elseStatement == node.ElseStatement)
                return node;

            return new BoundIfStatement(condition, thenStatement, elseStatement);
        }

        protected virtual BoundStatement RewriteForStatement(BoundForStatement node)
        {
            var lower = RewriteExpression(node.LowerBound);
            var upper = RewriteExpression(node.UpperBound);
            var body = RewriteStatement(node.Body);
            if (lower == node.LowerBound &&
                upper == node.UpperBound &&
                body == node.Body)
                return node;

            return new BoundForStatement(node.Variable, lower, upper, body);
        }

        protected virtual BoundStatement RewriteExpressionStatement(BoundExpressionStatement node)
        {
            var expression = RewriteExpression(node.Expression);
            if (expression == node.Expression)
                return node;

            return new BoundExpressionStatement(expression);
        }

        protected virtual BoundStatement RewriteBlockStatement(BoundBlockStatement node)
        {
            ImmutableArray<BoundStatement>.Builder builder = null;
            for (int i = 0; i < node.Statements.Length; i++)
            {
                var oldStatement = node.Statements[i];
                var newStatement = RewriteStatement(oldStatement);
                if (newStatement != oldStatement)
                {
                    if (builder == null)
                    {
                        builder = ImmutableArray.CreateBuilder<BoundStatement>(node.Statements.Length);
                        for (int j = 0; j < i; j++)
                            builder.Add(node.Statements[j]);
                    }
                }

                if (builder != null)
                    builder.Add(newStatement);
            }

            if (builder == null)
                return node;

            return new BoundBlockStatement(builder.MoveToImmutable());
        }

        protected virtual BoundExpression RewriteBinaryExpression(BoundBinaryExpression node)
        {
            var left = RewriteExpression(node.Left);
            var right = RewriteExpression(node.Right);
            if (left == node.Left && right == node.Right)
                return node;

            return new BoundBinaryExpression(left, node.Operator, right);
        }

        protected virtual BoundExpression RewriteUnaryExpression(BoundUnaryExpression node)
        {
            var operand = RewriteExpression(node.Operand);
            if (operand == node.Operand)
                return node;

            return new BoundUnaryExpression(node.Operator, operand);
        }

        protected virtual BoundExpression RewriteAssignmentExpression(BoundAssignmentExpression node)
        {
            var expression = RewriteExpression(node.Expression);
            if (expression == node.Expression)
                return node;

            return new BoundAssignmentExpression(node.Variable, expression);
        }

        protected virtual BoundExpression RewriteVariableExpression(BoundVariableExpression node)
        {
            return node;
        }

        protected virtual BoundExpression RewriteLiteralExpression(BoundLiteralExpression node)
        {
            return node;
        }
    }
}
