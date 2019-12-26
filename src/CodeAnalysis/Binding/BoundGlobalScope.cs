using CodeAnalysis.Binding.Nodes;
using CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace CodeAnalysis.Binding
{
    internal sealed class BoundGlobalScope
    {
        public BoundGlobalScope(
            ImmutableArray<Diagnostic> diagnostics,
            ImmutableArray<VariableSymbol> variables,
            BoundStatement statement,
            BoundGlobalScope previous)
        {
            Diagnostics = diagnostics;
            Variables = variables;
            Statement = statement;
            Previous = previous;
        }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ImmutableArray<VariableSymbol> Variables { get; }
        public BoundStatement Statement { get; }
        public BoundGlobalScope Previous { get; }
    }
}
