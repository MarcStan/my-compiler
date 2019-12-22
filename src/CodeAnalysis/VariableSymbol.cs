using System;

namespace CodeAnalysis
{
    public sealed class VariableSymbol
    {
        internal VariableSymbol(string name, bool isReadOnly, Type type)
        {
            Name = name;
            Type = type;
            IsReadOnly = isReadOnly;
        }

        internal VariableSymbol(string name, Type type)
            : this(name, false, type)
        {
        }

        public string Name { get; }
        public Type Type { get; }
        public bool IsReadOnly { get; }
    }
}
