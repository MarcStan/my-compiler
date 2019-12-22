﻿using CodeAnalysis.Binding;
using CodeAnalysis.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CodeAnalysis
{
    internal sealed class DiagnosticsCollection : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        private void Report(TextSpan span, string message)
        {
            _diagnostics.Add(new Diagnostic(span, message));
        }

        public void AddRange(DiagnosticsCollection diagnostics)
        {
            _diagnostics.AddRange(diagnostics);
        }

        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void ReportInvalidNumber(TextSpan span, string text, Type type)
            => Report(span, $"The number {text} isn't a valid {type}.");

        public void ReportBadCharacter(int position, char character)
            => Report(new TextSpan(position, 1), $"Error: bad character input: '{character}'.");

        public void ReportUnexpectedToken(TextSpan span, SyntaxKind actual, SyntaxKind expected)
            => Report(span, $"Unexpected token <{actual}>, expected <{expected}>.");

        public void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, BoundNodeKind operandKind)
            => Report(span, $"Unary operator '{operatorText}' is not defined for type {operandKind}.");

        public void ReportUndefinedBiaryOperator(TextSpan span, string operandText, Type leftType, Type rightType)
            => Report(span, $"Binary operator '{operandText}' is not defined for types {leftType} and {rightType}");
    }
}
