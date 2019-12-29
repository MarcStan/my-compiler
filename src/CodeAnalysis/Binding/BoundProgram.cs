using CodeAnalysis.Binding.Nodes;
using CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace CodeAnalysis.Binding
{
    internal sealed class BoundProgram
    {
        public BoundProgram(
            ImmutableArray<Diagnostic> diagnostics,
            ImmutableDictionary<FunctionSymbol, BoundBlockStatement> functionBodies,
            BoundBlockStatement statement)
        {
            Diagnostics = diagnostics;
            Functions = functionBodies;
            Statement = statement;
        }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ImmutableDictionary<FunctionSymbol, BoundBlockStatement> Functions { get; }
        public BoundBlockStatement Statement { get; }
    }
}
