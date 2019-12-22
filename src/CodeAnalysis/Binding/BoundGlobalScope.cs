using System.Collections.Immutable;

namespace CodeAnalysis.Binding
{
    internal sealed class BoundGlobalScope
    {
        public BoundGlobalScope(
            ImmutableArray<Diagnostic> diagnostics,
            ImmutableArray<VariableSymbol> variables,
            BoundExpression expression,
            BoundGlobalScope previous)
        {
            Diagnostics = diagnostics;
            Variables = variables;
            Expression = expression;
            Previous = previous;
        }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ImmutableArray<VariableSymbol> Variables { get; }
        public BoundExpression Expression { get; }
        public BoundGlobalScope Previous { get; }
    }
}
