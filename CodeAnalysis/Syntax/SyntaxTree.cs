using CodeAnalysis.Nodes.Syntax;
using System.Collections.Generic;

namespace CodeAnalysis.Syntax
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

        public static SyntaxTree Parse(string text)
        {
            var parser = new Parser(text);
            var syntaxTree = parser.Parse();

            return syntaxTree;
        }
    }
}
