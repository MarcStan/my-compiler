using CodeAnalysis.Syntax;
using FluentAssertions;
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
        [TestCase("1 == 2", false)]
        [TestCase("1 == 1", true)]
        [TestCase("9 != 9", false)]
        [TestCase("9 != 8", true)]
        [TestCase("9 == 8 + 1", true)]
        [TestCase("9 != 8 - 1", true)]
        [TestCase("(a = 10) * a", 100)]
        public void Expressions_should_evaluate_correctly(string text, object expected)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var comp = new Compilation(syntaxTree);

            var variables = new Dictionary<VariableSymbol, object>();

            var actual = comp.Evaluate(variables);

            actual.Diagnostics.Should().BeEmpty();
            actual.Value.Should().Be(expected);
        }
    }
}
