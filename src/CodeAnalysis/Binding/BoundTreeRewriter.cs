using CodeAnalysis.Binding.Nodes;
using System;
using System.Collections.Immutable;

namespace CodeAnalysis.Binding
{
    internal abstract class BoundTreeRewriter
    {
        public virtual BoundExpression RewriteExpression(BoundExpression node)
            => node.Kind switch
            {
                BoundNodeKind.ErrorExpression => RewriteErrorExpression((BoundErrorExpression)node),
                BoundNodeKind.LiteralExpression => RewriteLiteralExpression((BoundLiteralExpression)node),
                BoundNodeKind.VariableExpression => RewriteVariableExpression((BoundVariableExpression)node),
                BoundNodeKind.AssignmentExpression => RewriteAssignmentExpression((BoundAssignmentExpression)node),
                BoundNodeKind.UnaryExpression => RewriteUnaryExpression((BoundUnaryExpression)node),
                BoundNodeKind.BinaryExpression => RewriteBinaryExpression((BoundBinaryExpression)node),
                BoundNodeKind.CallExpression => RewriteCallExpression((BoundCallExpression)node),
                BoundNodeKind.ConversionExpression => RewriteConversionExpression((BoundConversionExpression)node),
                _ => throw new ArgumentException($"Cannot rewrite {node.Kind}")
            };

        public virtual BoundStatement RewriteStatement(BoundStatement node)
            => node.Kind switch
            {
                BoundNodeKind.BlockStatement => RewriteBlockStatement((BoundBlockStatement)node),
                BoundNodeKind.ExpressionStatement => RewriteExpressionStatement((BoundExpressionStatement)node),
                BoundNodeKind.ForStatement => RewriteForStatement((BoundForStatement)node),
                BoundNodeKind.IfStatement => RewriteIfStatement((BoundIfStatement)node),
                BoundNodeKind.VariableDeclaration => RewriteVariableDeclaration((BoundVariableDeclaration)node),
                BoundNodeKind.DoWhileStatement => RewriteDoWhileStatement((BoundDoWhileStatement)node),
                BoundNodeKind.WhileStatement => RewriteWhileStatement((BoundWhileStatement)node),
                BoundNodeKind.LabelStatement => RewriteLabelStatement((BoundLabelStatement)node),
                BoundNodeKind.GoToStatement => RewriteGoToStatement((BoundGoToStatement)node),
                BoundNodeKind.ReturnStatement => RewriteReturnStatement((BoundReturnStatement)node),
                BoundNodeKind.ConditionalGoToStatement => RewriteConditionalGoToStatement((BoundConditionalGoToStatement)node),
                _ => throw new ArgumentException($"Cannot rewrite {node.Kind}")
            };

        protected virtual BoundExpression RewriteConversionExpression(BoundConversionExpression node)
        {
            var expression = RewriteExpression(node.Expression);
            if (expression == node.Expression)
                return node;

            return new BoundConversionExpression(node.Type, expression);
        }

        protected virtual BoundExpression RewriteCallExpression(BoundCallExpression node)
        {
            ImmutableArray<BoundExpression>.Builder builder = null;
            for (int i = 0; i < node.Arguments.Length; i++)
            {
                var oldArg = node.Arguments[i];
                var newArg = RewriteExpression(oldArg);
                if (newArg != oldArg)
                {
                    if (builder == null)
                    {
                        builder = ImmutableArray.CreateBuilder<BoundExpression>(node.Arguments.Length);
                        for (int j = 0; j < i; j++)
                            builder.Add(node.Arguments[j]);
                    }
                }

                if (builder != null)
                    builder.Add(newArg);
            }

            if (builder == null)
                return node;

            return new BoundCallExpression(node.Function, builder.MoveToImmutable());
        }

        protected virtual BoundStatement RewriteReturnStatement(BoundReturnStatement node)
        {
            var expression = node.Expression == null ? null : RewriteExpression(node.Expression);
            if (expression == node.Expression)
                return node;

            return new BoundReturnStatement(expression);
        }

        protected virtual BoundStatement RewriteConditionalGoToStatement(BoundConditionalGoToStatement node)
        {
            var condition = RewriteExpression(node.Condition);
            if (condition == node.Condition)
                return node;

            return new BoundConditionalGoToStatement(node.Label, condition, node.JumpIfFalse);
        }

        protected virtual BoundExpression RewriteErrorExpression(BoundErrorExpression node) => node;

        protected virtual BoundStatement RewriteGoToStatement(BoundGoToStatement node) => node;

        protected virtual BoundStatement RewriteLabelStatement(BoundLabelStatement node) => node;

        protected virtual BoundStatement RewriteVariableDeclaration(BoundVariableDeclaration node)
        {
            var initializer = RewriteExpression(node.Initializer);
            if (initializer == node.Initializer)
                return node;

            return new BoundVariableDeclaration(node.Variable, initializer);
        }

        protected virtual BoundStatement RewriteDoWhileStatement(BoundDoWhileStatement node)
        {
            var body = RewriteStatement(node.Body);
            var condition = RewriteExpression(node.Condition);
            if (body == node.Body &&
                condition == node.Condition)
                return node;

            return new BoundDoWhileStatement(body, condition, node.BreakLabel, node.ContinueLabel);
        }

        protected virtual BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            var condition = RewriteExpression(node.Condition);
            var body = RewriteStatement(node.Body);
            if (condition == node.Condition &&
                body == node.Body)
                return node;

            return new BoundWhileStatement(condition, body, node.BreakLabel, node.ContinueLabel);
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

            return new BoundForStatement(node.Variable, lower, upper, body, node.BreakLabel, node.ContinueLabel);
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

        protected virtual BoundExpression RewriteVariableExpression(BoundVariableExpression node) => node;

        protected virtual BoundExpression RewriteLiteralExpression(BoundLiteralExpression node) => node;
    }
}
