using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Nodes;
using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CodeAnalysis.Lowering
{
    internal sealed class Lowerer : BoundTreeRewriter
    {
        private int _labelCount;

        private Lowerer()
        {
        }

        private BoundLabel GenerateLabel()
            => new BoundLabel($"label{++_labelCount}");

        public static BoundBlockStatement Lower(BoundStatement statement)
        {
            var lowerer = new Lowerer();
            return Flatten(lowerer.RewriteStatement(statement));
        }

        private static BoundBlockStatement Flatten(BoundStatement statement)
        {
            var builder = ImmutableArray.CreateBuilder<BoundStatement>();
            var stack = new Stack<BoundStatement>();
            stack.Push(statement);

            while (stack.Any())
            {
                var current = stack.Pop();

                if (current is BoundBlockStatement b)
                {
                    foreach (var s in b.Statements.Reverse())
                        stack.Push(s);
                }
                else
                {
                    builder.Add(current);
                }
            }

            return new BoundBlockStatement(builder.ToImmutable());
        }

        protected override BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            /* if <condition>
             *     <thenStatement>
             *
             *
             * ==>
             *
             * gotoIfFalse <condition> end
             *
             * <thenStatement>
             * end:
             */

            var endLabel = GenerateLabel();
            var endLabelStatement = new BoundLabelStatement(endLabel);

            if (node.ElseStatement == null)
            {
                var goToStatement = new BoundConditionalGoToStatement(endLabel, node.Condition, true);
                return RewriteStatement(new BoundBlockStatement(
                    ImmutableArray.Create(goToStatement, node.ThenStatement, endLabelStatement)));
            }

            /* if <condition>
             *     <thenStatement>
             * else
             *     <elseStatement>
             *
             * ==>
             *
             * gotoIfFalse <condition> else
             *
             * <thenStatement>
             * goto end
             * else:
             * <elseStatement>
             * end:
             */

            var elseLabel = GenerateLabel();
            var elseLabelStatement = new BoundLabelStatement(elseLabel);
            var goToElseStatement = new BoundConditionalGoToStatement(elseLabel, node.Condition, true);
            var goToEndStatement = new BoundGoToStatement(endLabel);
            return RewriteStatement(new BoundBlockStatement(
                ImmutableArray.Create(goToElseStatement, node.ThenStatement, goToEndStatement,
                                      elseLabelStatement, node.ElseStatement, endLabelStatement)));
        }

        protected override BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            /* while <condition>
             *     <body>
             *
             * ==>
             *
             * goto check
             * loop:
             *     <body>
             * check:
             * goToTrue <condition> loop
             * 
             */

            var checkLabel = GenerateLabel();
            var loopLabel = GenerateLabel();
            var goToCheckStatement = new BoundGoToStatement(checkLabel);
            var loopLabelStatement = new BoundLabelStatement(loopLabel);
            var checkLabelStatement = new BoundLabelStatement(checkLabel);
            var goToStatement = new BoundConditionalGoToStatement(loopLabel, node.Condition);

            return RewriteStatement(new BoundBlockStatement(
                ImmutableArray.Create(goToCheckStatement, loopLabelStatement, node.Body, checkLabelStatement, goToStatement)));
        }

        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {
            /*
             * for <var> = <lower> to <upper>
             *     <body>
             *
             *
             * ==>
             *
             * var <var> = <lower>
             * while <var> < <upper>
             *     <body>
             *     <var> = <var> + 1
             */
            var variableDeclaration = new BoundVariableDeclaration(node.Variable, node.LowerBound);
            var variableExpression = new BoundVariableExpression(node.Variable);

            var upperBoundSymbol = new VariableSymbol("upperBound", true, typeof(int));
            var upperBoundDeclaration = new BoundVariableDeclaration(upperBoundSymbol, node.UpperBound);
            var condition = new BoundBinaryExpression(
                variableExpression,
                BoundBinaryOperator.Bind(SyntaxKind.LessOrEqualsToken, typeof(int), typeof(int)),
                new BoundVariableExpression(upperBoundSymbol));

            var increment = new BoundExpressionStatement(
                new BoundAssignmentExpression(
                    node.Variable,
                    new BoundBinaryExpression(
                        variableExpression,
                        BoundBinaryOperator.Bind(SyntaxKind.PlusToken, typeof(int), typeof(int)),
                        new BoundLiteralExpression(1))));

            var whileBody = new BoundBlockStatement(ImmutableArray.Create(node.Body, increment));

            var whileStatement = new BoundWhileStatement(condition, whileBody);

            var result = new BoundBlockStatement(
                ImmutableArray.Create<BoundStatement>(
                    variableDeclaration,
                    upperBoundDeclaration,
                    whileStatement));
            return RewriteStatement(result);
        }
    }
}
