using CodeAnalysis.Binding;
using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;

namespace CodeAnalysis
{
    public sealed class Compilation
    {
        private BoundGlobalScope _globalScope;
        private readonly Compilation _previous;

        public Compilation(SyntaxTree syntaxTree, Compilation previous)
        {
            Syntax = syntaxTree;
            _previous = previous;
        }

        public Compilation(SyntaxTree syntaxTree)
            : this(syntaxTree, null)
        {
        }

        public SyntaxTree Syntax { get; }

        internal BoundGlobalScope GlobalScope
        {
            get
            {
                if (_globalScope == null)
                {
                    var globalScope = Binder.BindGlobalScope(Syntax.Root, _previous?.GlobalScope);
                    Interlocked.CompareExchange(ref _globalScope, globalScope, null);
                }
                return _globalScope;
            }
        }

        public Compilation ContinueWith(SyntaxTree syntaxTree)
            => new Compilation(syntaxTree, this);

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {
            var diag = Syntax.Diagnostics
                .Concat(GlobalScope.Diagnostics)
                .ToImmutableArray();

            if (diag.Any())
                return new EvaluationResult(diag, null);

            var program = Binder.BindProgram(GlobalScope);
            if (program.Diagnostics.Any())
                return new EvaluationResult(program.Diagnostics, null);

            var evaluator = new Evaluator(program, variables);
            var value = evaluator.Evaluate();

            return new EvaluationResult(diag, value);
        }

        public void EmitTree(TextWriter writer)
        {
            var program = Binder.BindProgram(GlobalScope);
            program.Statement.WriteTo(writer);
        }
    }
}
