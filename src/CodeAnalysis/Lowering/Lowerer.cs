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
             * break:
             */

            var checkLabel = GenerateLabel();

            var goToCheck = new BoundGoToStatement(checkLabel);
            var continueLabelStatement = new BoundLabelStatement(node.ContinueLabel);
            var checkLabelStatement = new BoundLabelStatement(checkLabel);
            var goToStatement = new BoundConditionalGoToStatement(node.ContinueLabel, node.Condition);
            var breakLabelStatement = new BoundLabelStatement(node.BreakLabel);

            return RewriteStatement(new BoundBlockStatement(ImmutableArray.Create(
                    goToCheck,
                    continueLabelStatement,
                    node.Body,
                    checkLabelStatement,
                    goToStatement,
                    breakLabelStatement)));
        }

        protected override BoundStatement RewriteDoWhileStatement(BoundDoWhileStatement node)
        {
            /* do
             *     <body>
             * while <condition>
             *
             * ==>
             *
             * loop:
             *     <body>
             * goToTrue <condition> loop
             * break:
             */
            var loopLabelStatement = new BoundLabelStatement(node.ContinueLabel);
            var goToStatement = new BoundConditionalGoToStatement(node.ContinueLabel, node.Condition);
            var breakLabelStatement = new BoundLabelStatement(node.BreakLabel);

            return RewriteStatement(new BoundBlockStatement(ImmutableArray.Create(
                loopLabelStatement,
                node.Body,
                goToStatement,
                breakLabelStatement)));
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

            var upperBoundSymbol = new LocalVariableSymbol("upperBound", true, TypeSymbol.Int);
            var upperBoundDeclaration = new BoundVariableDeclaration(upperBoundSymbol, node.UpperBound);
            var condition = new BoundBinaryExpression(
                variableExpression,
                BoundBinaryOperator.Bind(SyntaxKind.LessOrEqualsToken, TypeSymbol.Int, TypeSymbol.Int),
                new BoundVariableExpression(upperBoundSymbol));

            var continueLabelStatement = new BoundLabelStatement(node.ContinueLabel);
            var increment = new BoundExpressionStatement(
                new BoundAssignmentExpression(
                    node.Variable,
                    new BoundBinaryExpression(
                        variableExpression,
                        BoundBinaryOperator.Bind(SyntaxKind.PlusToken, TypeSymbol.Int, TypeSymbol.Int),
                        new BoundLiteralExpression(1))));

            var whileBody = new BoundBlockStatement(ImmutableArray.Create(node.Body, continueLabelStatement, increment));

            var whileStatement = new BoundWhileStatement(condition, whileBody, node.BreakLabel, GenerateLabel());

            var result = new BoundBlockStatement(
                ImmutableArray.Create<BoundStatement>(
                    variableDeclaration,
                    upperBoundDeclaration,
                    whileStatement));
            return RewriteStatement(result);
        }
    }
}
