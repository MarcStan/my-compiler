using System;

namespace mc
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("> ");

                var line = Console.ReadLine();

                var parser = new Parser(line);

                var expression = parser.Parse();

                Print(expression);
            }
        }

        private static void Print(SyntaxNode node, string indent = "")
        {
            Console.Write(indent);
            Console.Write(node.Kind);

            if (node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indent += "  ";

            foreach (var c in node.Children)
                Print(c, indent);
        }
    }
}
