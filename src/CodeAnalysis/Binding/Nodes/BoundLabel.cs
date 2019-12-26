namespace CodeAnalysis.Binding.Nodes
{
    internal sealed class BoundLabel
    {
        public BoundLabel(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
