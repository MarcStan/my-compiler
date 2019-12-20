using CodeAnalysis.Syntax;
using System;
using System.Linq;

namespace CodeAnalysis.Binding
{
    internal sealed class BoundBinaryOperator
    {
        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type leftType, Type rightType, Type resultType)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            LeftType = leftType;
            RightType = rightType;
            ResultType = resultType;
        }

        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type type)
            : this(syntaxKind, kind, type, type, type)
        {
        }

        public SyntaxKind SyntaxKind { get; }

        public BoundBinaryOperatorKind Kind { get; }

        public Type LeftType { get; }

        public Type RightType { get; }

        public Type ResultType { get; }

        private static BoundBinaryOperator[] _operators =
        {
            new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, typeof(int)),

            new BoundBinaryOperator(SyntaxKind.AmpersandToken, BoundBinaryOperatorKind.LogicalAdd, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.PipeToken, BoundBinaryOperatorKind.LogicalOr, typeof(bool))
        };

        public static BoundBinaryOperator Bind(SyntaxKind syntaxKind, Type leftType, Type rightType)
            => _operators.FirstOrDefault(o =>
                o.SyntaxKind == syntaxKind &&
                o.LeftType == leftType &&
                o.RightType == rightType
            );
    }
}
