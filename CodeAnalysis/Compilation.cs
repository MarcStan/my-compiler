using CodeAnalysis.Binding;
using CodeAnalysis.Syntax;
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

        public EvaluationResult Evaluate()
        {
            var binder = new Binder();
            var boundExpression = binder.BindExpression(Syntax.Root);

            var diag = Syntax.Diagnostics
                .Concat(binder.Diagnostics)
                .ToArray();

            if (diag.Any())
                return new EvaluationResult(diag, null);

            var evaluator = new Evaluator(boundExpression);
            var value = evaluator.Evaluate();

            return new EvaluationResult(diag, value);
        }
    }
}
