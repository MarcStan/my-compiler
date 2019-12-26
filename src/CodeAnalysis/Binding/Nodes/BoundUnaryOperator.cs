﻿using CodeAnalysis.Syntax;
using System;
using System.Linq;

namespace CodeAnalysis.Binding.Nodes
{
    internal sealed class BoundUnaryOperator
    {
        private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, Type operandType, Type resultType)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            OperandType = operandType;
            Type = resultType;
        }

        private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, Type operandType)
            : this(syntaxKind, kind, operandType, operandType)
        {
        }

        public SyntaxKind SyntaxKind { get; }

        public BoundUnaryOperatorKind Kind { get; }

        public Type OperandType { get; }

        public Type Type { get; }

        private static BoundUnaryOperator[] _operators =
        {
            new BoundUnaryOperator(SyntaxKind.BangToken, BoundUnaryOperatorKind.LogicalNegation, typeof(bool)),
            new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, typeof(int)),
            new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, typeof(int)),
            new BoundUnaryOperator(SyntaxKind.TildeToken, BoundUnaryOperatorKind.OnesComplement, typeof(int)),
        };

        public static BoundUnaryOperator Bind(SyntaxKind syntaxKind, Type operandType)
            => _operators.FirstOrDefault(o => o.SyntaxKind == syntaxKind && o.OperandType == operandType);
    }
}