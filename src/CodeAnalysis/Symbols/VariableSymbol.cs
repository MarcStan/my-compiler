namespace CodeAnalysis.Symbols
{
    public abstract class VariableSymbol : Symbol
    {
        internal VariableSymbol(string name, bool isReadOnly, TypeSymbol type)
            : base(name)
        {
            Type = type;
            IsReadOnly = isReadOnly;
        }

        public TypeSymbol Type { get; }
        public bool IsReadOnly { get; }
    }
}
