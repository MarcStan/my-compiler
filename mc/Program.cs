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

                var lexer = new Lexer(line);
                while (true)
                {
                    var token = lexer.GetNextToken();
                    if (token.Kind == SyntaxKind.EndOfFileToken)
                        break;

                    Console.Write($"{token.Kind}: '{token.Text}'");
                    if (token.Value != null)
                        Console.Write($"  {token.Value}");

                    Console.WriteLine();
                }

                if (line == "exit")
                    return;
            }
        }
    }
}
