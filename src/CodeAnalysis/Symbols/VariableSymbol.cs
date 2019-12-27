using System;

namespace CodeAnalysis.Symbols
{
    public sealed class VariableSymbol : Symbol
    {
        internal VariableSymbol(string name, bool isReadOnly, Type type)
            : base(name)
        {
            Type = type;
            IsReadOnly = isReadOnly;
        }

        public Type Type { get; }
        public bool IsReadOnly { get; }
        public override SymbolKind Kind => SymbolKind.Variable;
    }
}
