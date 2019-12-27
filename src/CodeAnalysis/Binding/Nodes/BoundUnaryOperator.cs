using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using System.Linq;

namespace CodeAnalysis.Binding.Nodes
{
    internal sealed class BoundUnaryOperator
    {
        private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, TypeSymbol operandType, TypeSymbol resultType)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            OperandType = operandType;
            Type = resultType;
        }

        private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, TypeSymbol operandType)
            : this(syntaxKind, kind, operandType, operandType)
        {
        }

        public SyntaxKind SyntaxKind { get; }

        public BoundUnaryOperatorKind Kind { get; }

        public TypeSymbol OperandType { get; }

        public TypeSymbol Type { get; }

        private static BoundUnaryOperator[] _operators =
        {
            new BoundUnaryOperator(SyntaxKind.BangToken, BoundUnaryOperatorKind.LogicalNegation, TypeSymbol.Bool),
            new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, TypeSymbol.Int),
            new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, TypeSymbol.Int),
            new BoundUnaryOperator(SyntaxKind.TildeToken, BoundUnaryOperatorKind.OnesComplement, TypeSymbol.Int),
        };

        public static BoundUnaryOperator Bind(SyntaxKind syntaxKind, TypeSymbol operandType)
            => _operators.FirstOrDefault(o => o.SyntaxKind == syntaxKind && o.OperandType == operandType);
    }
}
