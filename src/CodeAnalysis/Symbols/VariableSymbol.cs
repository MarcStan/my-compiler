namespace CodeAnalysis.Symbols
{
    public class VariableSymbol : Symbol
    {
        internal VariableSymbol(string name, bool isReadOnly, TypeSymbol type)
            : base(name)
        {
            Type = type;
            IsReadOnly = isReadOnly;
        }

        public TypeSymbol Type { get; }
        public bool IsReadOnly { get; }
        public override SymbolKind Kind => SymbolKind.Variable;
    }
}
