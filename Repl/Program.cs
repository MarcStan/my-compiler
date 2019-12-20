using CodeAnalysis;
using CodeAnalysis.Binding;
using CodeAnalysis.Syntax;
using System;
using System.Linq;

namespace Repl
{
    public static class Program
    {
        public static void Main()
        {
            bool showTree = false;
            while (true)
            {
                Console.Write("> ");

                var line = Console.ReadLine();

                if (line == "showTree")
                {
                    showTree = !showTree;
                    Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees");
                    continue;
                }
                else if (line == "cls")
                {
                    Console.Clear();
                    continue;
                }
                else if (line == "exit")
                {
                    break;
                }
                var syntaxTree = SyntaxTree.Parse(line);
                var binder = new Binder();
                var boundExpression = binder.BindExpression(syntaxTree.Root);

                var diagnostics = syntaxTree.Diagnostics
                    .Concat(binder.Diagnostics)
                    .ToArray();

                if (showTree)
                    Print(syntaxTree.Root);

                if (diagnostics.Any())
                {
                    WriteLine(ConsoleColor.Red, string.Join(Environment.NewLine, diagnostics));
                }
                else
                {
                    var eval = new Evaluator(boundExpression);
                    var result = eval.Evaluate();
                    Console.WriteLine(result);
                }
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
