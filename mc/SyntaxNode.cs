using System.Collections.Generic;

namespace mc
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> Children { get; }
    }
}
