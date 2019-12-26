using System;

namespace CodeAnalysis.Binding.Nodes
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract Type Type { get; }
    }
}
