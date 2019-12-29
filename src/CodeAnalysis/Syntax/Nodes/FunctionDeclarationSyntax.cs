namespace CodeAnalysis.Syntax.Nodes
{
    public sealed class FunctionDeclarationSyntax : MemberSyntax
    {
        public FunctionDeclarationSyntax(
            SyntaxToken functionKeyword,
            SyntaxToken identifier,
            SyntaxToken openParenthesis,
            SeparatedSyntaxList<ParameterSyntax> parameters,
            SyntaxToken closeParenthesis,
            TypeClauseSyntax type,
            StatementSyntax body)
        {
            FunctionKeyword = functionKeyword;
            Identifier = identifier;
            OpenParenthesis = openParenthesis;
            Parameters = parameters;
            CloseParenthesis = closeParenthesis;
            Type = type;
            Body = body;
        }

        public override SyntaxKind Kind => SyntaxKind.FunctionDeclaration;

        public SyntaxToken FunctionKeyword { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken OpenParenthesis { get; }
        public SeparatedSyntaxList<ParameterSyntax> Parameters { get; }
        public SyntaxToken CloseParenthesis { get; }
        public TypeClauseSyntax Type { get; }
        public StatementSyntax Body { get; }
    }
}
