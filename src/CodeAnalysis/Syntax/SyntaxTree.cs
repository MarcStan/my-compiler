using CodeAnalysis.Syntax.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace CodeAnalysis.Syntax
{
    public class SyntaxTree
    {
        public SyntaxTree(ExpressionSyntax root, SyntaxToken endOfFile, IEnumerable<Diagnostic> diagnostics)
        {
            Root = root;
            EndOfFile = endOfFile;
            Diagnostics = diagnostics.ToArray();
        }

        public SyntaxToken EndOfFile { get; }

        public ExpressionSyntax Root { get; }
        public IReadOnlyList<Diagnostic> Diagnostics { get; }

        public static SyntaxTree Parse(string text)
        {
            var parser = new Parser(text);
            var syntaxTree = parser.Parse();

            return syntaxTree;
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            var lexer = new Lexer(text);
            while (true)
            {
                var token = lexer.Lex();
                if (token.Kind == SyntaxKind.EndOfFileToken)
                    break;

                yield return token;
            }
        }
    }
}
