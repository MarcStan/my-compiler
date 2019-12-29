using CodeAnalysis.Binding.Nodes;
using CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace CodeAnalysis.Binding
{
    internal sealed class BoundGlobalScope
    {
        public BoundGlobalScope(
            ImmutableArray<Diagnostic> diagnostics,
            ImmutableArray<FunctionSymbol> functions,
            ImmutableArray<VariableSymbol> variables,
            ImmutableArray<BoundStatement> statements,
            BoundGlobalScope previous)
        {
            Diagnostics = diagnostics;
            Functions = functions;
            Variables = variables;
            Statements = statements;
            Previous = previous;
        }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ImmutableArray<FunctionSymbol> Functions { get; }
        public ImmutableArray<VariableSymbol> Variables { get; }
        public ImmutableArray<BoundStatement> Statements { get; }
        public BoundGlobalScope Previous { get; }
    }
}
