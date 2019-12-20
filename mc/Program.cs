using System;
using System.Linq;

namespace mc
{
    public static class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("> ");

                var line = Console.ReadLine();

                var parser = new Parser(line);

                var syntaxTree = parser.Parse();

                if (syntaxTree.Diagnostics.Any())
                    WriteLine(ConsoleColor.Red, string.Join(Environment.NewLine, syntaxTree.Diagnostics));

                Print(syntaxTree.Root);
            }
        }

        private static void WriteLine(ConsoleColor color, string text)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = old;
        }

        private static void Print(SyntaxNode node, string indent = "", bool isLast = true)
        {
            // ├──
            // └─
            // │

            var marker = isLast ? "└─" : "├─";
            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);

            if (node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indent += isLast ? "  " : "│ ";

            var last = node.Children.LastOrDefault();

            foreach (var c in node.Children)
                Print(c, indent, c == last);
        }
    }
}
