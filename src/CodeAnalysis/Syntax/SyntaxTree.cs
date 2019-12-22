using CodeAnalysis.Syntax.Nodes;
using CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CodeAnalysis.Syntax
{
    public class SyntaxTree
    {
        public SyntaxTree(SourceText text)
        {
            var parser = new Parser(text);
            var root = parser.ParseCompilationUnit();
            var diagnostics = parser.Diagnostics.ToImmutableArray();

            Text = text;
            Diagnostics = diagnostics;
            Root = root;
        }

        public CompilationUnitSyntax Root { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public SourceText Text { get; }

        public static SyntaxTree Parse(string text)
            => Parse(SourceText.From(text));

        public static SyntaxTree Parse(SourceText text)
            => new SyntaxTree(text);

        public static IEnumerable<SyntaxToken> ParseTokens(string text)
            => ParseTokens(SourceText.From(text));

        public static IEnumerable<SyntaxToken> ParseTokens(SourceText text)
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
