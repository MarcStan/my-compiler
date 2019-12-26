namespace CodeAnalysis.Binding.Nodes
{
    internal sealed class LabelSymbol
    {
        public LabelSymbol(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
