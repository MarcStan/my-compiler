using CodeAnalysis;
using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repl
{
    public static class Program
    {
        public static void Main()
        {
            bool showTree = false;
            bool showProgram = false;
            var variables = new Dictionary<VariableSymbol, object>();
            var builder = new StringBuilder();

            Compilation previous = null;
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (builder.Length == 0)
                    Console.Write("> ");
                else
                    Console.Write("| ");

                Console.ResetColor();
                var input = Console.ReadLine();

                var isBlank = string.IsNullOrEmpty(input);
                if (builder.Length == 0)
                {
                    if (isBlank)
                        break;
                    else if (input == "showTree")
                    {
                        showTree = !showTree;
                        Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees");
                        continue;
                    }
                    else if (input == "showProgram")
                    {
                        showProgram = !showProgram;
                        Console.WriteLine(showProgram ? "Showing bound tree." : "Not showing bound tree");
                        continue;
                    }
                    else if (input == "cls")
                    {
                        Console.Clear();
                        continue;
                    }
                    else if (input == "reset")
                    {
                        previous = null;
                        continue;
                    }
                }
                builder.AppendLine(input);
                var text = builder.ToString();
                var syntaxTree = SyntaxTree.Parse(text);
                if (!isBlank && syntaxTree.Diagnostics.Any())
                    continue;

                builder.Clear();
                var comp = previous == null ?
                    new Compilation(syntaxTree) :
                    previous.ContinueWith(syntaxTree);

                var result = comp.Evaluate(variables);

                if (!result.Diagnostics.Any())
                {
                    previous = comp;

                    if (result.Value != null)
                        Console.WriteLine(result.Value);
                }
                else
                {
                    foreach (var diag in result.Diagnostics)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        var lineIndex = syntaxTree.Text.GetLineIndex(diag.Span.Start);
                        var lineNumber = lineIndex + 1;
                        var line = syntaxTree.Text.Lines[lineIndex];
                        var character = diag.Span.Start - line.Start + 1;
                        Console.Write($"({lineNumber}, {character}): ");
                        Console.WriteLine(diag);
                        Console.ResetColor();

                        var prefixSpan = TextSpan.FromBounds(line.Start, diag.Span.Start);
                        var suffixSpan = TextSpan.FromBounds(diag.Span.End, line.End);
                        var prefix = syntaxTree.Text.ToString(prefixSpan);
                        var error = syntaxTree.Text.ToString(diag.Span);
                        var suffix = syntaxTree.Text.ToString(suffixSpan);

                        Console.Write("  ");
                        Console.Write(prefix);

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(error);
                        Console.ResetColor();

                        Console.WriteLine(suffix);
                    }
                }

                if (showTree)
                    syntaxTree.Root.WriteTo(Console.Out);
                if (showProgram)
                    comp.EmitTree(Console.Out);
            }
        }
    }
}
