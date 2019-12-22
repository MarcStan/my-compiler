using CodeAnalysis.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public virtual TextSpan Span
            => TextSpan.FromBounds(Children.First().Span.Start, Children.Last().Span.End);

        public abstract IEnumerable<SyntaxNode> Children { get; }

        public void WriteTo(TextWriter writer)
            => PrintTree(writer, this);

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }

        private static void PrintTree(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
        {
            var marker = isLast ? "└─" : "├─";
            writer.Write(indent);
            writer.Write(marker);
            writer.Write(node.Kind);

            if (node is SyntaxToken t && t.Value != null)
            {
                writer.Write(" ");
                writer.Write(t.Value);
            }

            writer.WriteLine();

            indent += isLast ? "  " : "│ ";

            var last = node.Children.LastOrDefault();

            foreach (var c in node.Children)
                PrintTree(writer, c, indent, c == last);
        }
    }
}
