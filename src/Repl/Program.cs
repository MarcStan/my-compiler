using CodeAnalysis;
using CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repl
{
    public static class Program
    {
        public static void Main()
        {
            bool showTree = false;
            var variables = new Dictionary<VariableSymbol, object>();
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
                var comp = new Compilation(syntaxTree);

                var result = comp.Evaluate(variables);
                if (result.Diagnostics.Any())
                {
                    foreach (var diag in result.Diagnostics)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(diag);
                        Console.ResetColor();

                        var prefix = line.Substring(0, diag.Span.Start);
                        var error = line.Substring(diag.Span.Start, diag.Span.Length);
                        var suffix = line.Substring(diag.Span.End);

                        Console.Write("  ");
                        Console.Write(prefix);

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(error);
                        Console.ResetColor();

                        Console.WriteLine(suffix);
                    }
                }
                else
                {
                    if (showTree)
                        Print(syntaxTree.Root);

                    Console.WriteLine(result.Value);
                }
            }
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
