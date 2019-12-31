using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;
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

        public void ReportInvalidNumber(TextSpan span, string text, TypeSymbol type)
            => Report(span, $"The number {text} isn't a valid {type}.");

        public void ReportBadCharacter(int position, char character)
            => Report(new TextSpan(position, 1), $"Error: bad character input: '{character}'.");

        public void ReportUnexpectedToken(TextSpan span, SyntaxKind actual, SyntaxKind expected)
            => Report(span, $"Unexpected token <{actual}>, expected <{expected}>.");

        public void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, TypeSymbol operandType)
            => Report(span, $"Unary operator '{operatorText}' is not defined for type '{operandType}'.");

        public void ReportUndefinedBiaryOperator(TextSpan span, string operandText, TypeSymbol leftType, TypeSymbol rightType)
            => Report(span, $"Binary operator '{operandText}' is not defined for types '{leftType}' and '{rightType}'.");

        public void ReportUndefinedName(TextSpan span, string name)
            => Report(span, $"Variable '{name}' does not exist.");

        public void ReportExpressionMustHaveValue(TextSpan span)
            => Report(span, "Expression must have a value.");

        public void ReportVariableAlreadyDeclared(TextSpan span, string name)
            => Report(span, $"Variable '{name}' already declared.");

        public void ReportCannotConvert(TextSpan span, TypeSymbol fromType, TypeSymbol toType)
            => Report(span, $"Cannot convert type '{fromType}' to '{toType}'.");

        public void ReportParameterAlreadyDeclared(TextSpan span, string name)
            => Report(span, $"Parameter {name} declared multiple times.");

        public void ReportCannotAssign(TextSpan span, string name)
            => Report(span, $"Variable '{name}' is read-only and cannot be assigned to.");

        public void ReportUnterminatedString(TextSpan span)
            => Report(span, "Unterminated string literal.");

        public void ReportFunctionAlreadyDeclared(TextSpan span, string name)
            => Report(span, $"Function '{name}' already declared.");

        public void ReportUndefinedFunction(TextSpan span, string text)
            => Report(span, $"Undefined function '{text}'.");

        public void ReportAllPathsMustReturn(TextSpan span)
            => Report(span, "Not all path return a value.");

        public void ReportWrongArgumentCount(TextSpan span, string name, int expected, int actual)
            => Report(span, $"Function '{name}' requires {expected} arguments but was given {actual}.");

        public void ReportWrongArgumentType(TextSpan span, string functionName, string argName, TypeSymbol expected, TypeSymbol actual)
            => Report(span, $"Function '{functionName}' requires argument '{argName}' to be of type {expected} but received {actual}.");

        public void ReportUndefinedType(TextSpan span, string name)
            => Report(span, $"Type '{name}' does not exist.");

        public void ReportInvalidBreakOrContinue(TextSpan span, string text)
            => Report(span, $"The keyword {text} can only be used in loops.");

        public void ReportInvalidReturnExpression(TextSpan span, string text)
            => Report(span, $"Method '{text}' is of type void and cannot return value.");

        public void ReportMissingReturnExpression(TextSpan span, TypeSymbol type)
            => Report(span, $"Missing {type} return value.");

        public void ReportInvalidReturn(TextSpan span)
            => Report(span, "Return keyword is invalid outside methods.");
    }
}
