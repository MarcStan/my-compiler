using mc.Nodes;
using System.Collections.Generic;

namespace mc
{
    public class SyntaxTree
    {
        public SyntaxTree(ExpressionSyntax root, SyntaxToken endOfFile, IReadOnlyList<string> diagnostics)
        {
            Root = root;
            EndOfFile = endOfFile;
            Diagnostics = diagnostics;
        }

        public SyntaxToken EndOfFile { get; }

        public ExpressionSyntax Root { get; }
        public IReadOnlyList<string> Diagnostics { get; }
    }
}
