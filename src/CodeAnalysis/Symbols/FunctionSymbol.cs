using CodeAnalysis.Syntax.Nodes;
using System.Collections.Immutable;

namespace CodeAnalysis.Symbols
{
    public sealed class FunctionSymbol : Symbol
    {
        public FunctionSymbol(
            string name,
            ImmutableArray<ParameterSymbol> parameters,
            TypeSymbol returnType,
            FunctionDeclarationSyntax declaration = null)
            : base(name)
        {
            Parameters = parameters;
            ReturnType = returnType;
            Declaration = declaration;
        }

        public override SymbolKind Kind => SymbolKind.Function;

        public FunctionDeclarationSyntax Declaration { get; }
        public ImmutableArray<ParameterSymbol> Parameters { get; }
        public TypeSymbol ReturnType { get; }
    }
}
