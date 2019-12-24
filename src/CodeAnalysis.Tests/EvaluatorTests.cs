using CodeAnalysis.Syntax;
using CodeAnalysis.Tests.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;
using System.Collections.Generic;

namespace CodeAnalysis.Tests
{
    public class EvaluatorTests
    {
        [TestCase("1", 1)]
        [TestCase("+1", 1)]
        [TestCase("-1", -1)]
        [TestCase("1+2", 3)]
        [TestCase("1-2", -1)]
        [TestCase("3*2", 6)]
        [TestCase("9/3", 3)]
        [TestCase("9/3", 3)]
        [TestCase("(1+1)", 2)]
        [TestCase("1 + 2 * 3", 7)]
        [TestCase("(1 + 2) * 3", 9)]
        [TestCase("true", true)]
        [TestCase("false", false)]
        [TestCase("!false", true)]
        [TestCase("!true", false)]
        [TestCase("true || true", true)]
        [TestCase("true || false", true)]
        [TestCase("false || false", false)]
        [TestCase("false && false", false)]
        [TestCase("false && true", false)]
        [TestCase("true && true", true)]
        [TestCase("3 < 4", true)]
        [TestCase("3 < 3", false)]
        [TestCase("3 <= 3", true)]
        [TestCase("4 <= 3", false)]
        [TestCase("4 > 3", true)]
        [TestCase("4 > 5", false)]
        [TestCase("4 >= 4", true)]
        [TestCase("3 >= 4", false)]
        [TestCase("3 >= 4", false)]
        [TestCase("1 == 2", false)]
        [TestCase("1 == 1", true)]
        [TestCase("9 != 9", false)]
        [TestCase("9 != 8", true)]
        [TestCase("9 == 8 + 1", true)]
        [TestCase("9 != 8 - 1", true)]
        [TestCase("{ var a = 0 (a = 10) * a }", 100)]
        [TestCase("{ var a = 10 if a == 10 a }", 10)]
        [TestCase("{ var a = 5 if a == 10 a a }", 5)]
        [TestCase("{ var a = 5 if a == 10 a else a * 3 }", 15)]
        [TestCase("{ var a = 10 if a == 10 a else a * 3 }", 10)]
        [TestCase("{ var i = 0 var a = 0 while i < 10 { a = a + 2 i = i + 1 } a }", 20)]
        [TestCase("{ var a = 0 for i = 0 to 10 { a = i + 1 } a }", 10)]
        public void Expressions_should_evaluate_correctly(string text, object expected)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var comp = new Compilation(syntaxTree);

            var variables = new Dictionary<VariableSymbol, object>();

            var actual = comp.Evaluate(variables);

            actual.Diagnostics.Should().BeEmpty();
            actual.Value.Should().Be(expected);
        }

        [Test]
        public void Variable_declaration_should_report_redeclaration()
        {
            var text = @"
                {
                    var x = 10
                    var y = 10
                    {
                        var x = 1
                    }
                    var [x] = 5
                }
            ";

            var diagnostics = @"
                Variable 'x' already declared.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Variable_assignment_should_cause_error()
        {
            var text = @"
                {
                    let x = 10
                    x [=] 0
                }
            ";

            var diagnostics = @"
                Variable 'x' is read-only and cannot be assigned to.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Variable_assignment_with_changed_type_should_cause_error()
        {
            var text = @"
                {
                    var x = 10
                    x = [true]
                }
            ";

            var diagnostics = @"
                Cannot convert type 'System.Boolean' to 'System.Int32'.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Unary_operator_on_boolean_is_undefined_and_should_cause_error()
        {
            var text = @"[+]true";

            var diagnostics = @"
                Unary operator '+' is not defined for type 'System.Boolean'.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Binary_operator_on_int_and_boolean_is_undefined_and_should_cause_error()
        {
            var text = @"10 [*] true";

            var diagnostics = @"
                Binary operator '*' is not defined for types 'System.Int32' and 'System.Boolean'.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Test]
        public void Undefined_variable_use_should_report_missing()
        {
            var text = @"[x] * 10";

            var diagnostics = @"
                Variable 'x' does not exist.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        private void AssertDiagnostics(string text, string diagnosticText)
        {
            var annotatedText = AnnotatedText.Parse(text);
            var tree = SyntaxTree.Parse(annotatedText.Text);

            var comp = new Compilation(tree);
            var result = comp.Evaluate(new Dictionary<VariableSymbol, object>());

            var diagnostics = AnnotatedText.UnindentLines(diagnosticText);

            annotatedText.Spans.Should().HaveCount(diagnostics.Length);

            for (int i = 0; i < diagnostics.Length; i++)
            {
                var expectedMessage = diagnostics[i];
                var actualMessage = result.Diagnostics[i].Message;

                actualMessage.Should().Be(expectedMessage);

                var expected = annotatedText.Spans[i];
                var actual = result.Diagnostics[i].Span;
                actual.Should().Be(expected);
            }
        }
    }
}
