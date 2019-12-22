using CodeAnalysis.Binding;
using CodeAnalysis.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace CodeAnalysis
{
    public sealed class Compilation
    {
        public Compilation(SyntaxTree syntaxTree)
        {
            Syntax = syntaxTree;
        }

        public SyntaxTree Syntax { get; }

        public EvaluationResult Evaluate(Dictionary<string, object> variables)
        {
            var binder = new Binder(variables);
            var boundExpression = binder.BindExpression(Syntax.Root);

            var diag = Syntax.Diagnostics
                .Concat(binder.Diagnostics)
                .ToArray();

            if (diag.Any())
                return new EvaluationResult(diag, null);

            var evaluator = new Evaluator(boundExpression, variables);
            var value = evaluator.Evaluate();

            return new EvaluationResult(diag, value);
        }
    }
}
