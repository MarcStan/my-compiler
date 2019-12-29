using CodeAnalysis.Syntax.Nodes;
using CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CodeAnalysis.Syntax
{
    internal class Parser
    {
        private readonly ImmutableArray<SyntaxToken> _tokens;
        private int _position;
        private readonly SourceText _text;

        public Parser(SourceText text)
        {
            var lexer = new Lexer(text);
            SyntaxToken token;

            var tokens = new List<SyntaxToken>();
            do
            {
                token = lexer.Lex();
                if (token.Kind != SyntaxKind.Whitespace &&
                    token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndOfFileToken);

            _tokens = tokens.ToImmutableArray();
            Diagnostics.AddRange(lexer.Diagnostics);
            _text = text;
        }

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _tokens.Length)
                return _tokens[^1];
            return _tokens[index];
        }

        public SyntaxToken Current
            => Peek(0);

        public DiagnosticsCollection Diagnostics { get; } = new DiagnosticsCollection();

        private SyntaxToken GetNextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        private SyntaxToken MatchToken(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return GetNextToken();

            Diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, kind);
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        public CompilationUnitSyntax ParseCompilationUnit()
        {
            var members = ParseMembers();
            var eof = MatchToken(SyntaxKind.EndOfFileToken);
            return new CompilationUnitSyntax(members, eof);
        }

        private ImmutableArray<MemberSyntax> ParseMembers()
        {
            var members = ImmutableArray.CreateBuilder<MemberSyntax>();

            while (Current.Kind != SyntaxKind.EndOfFileToken)
            {
                var startToken = Current;

                members.Add(ParseMember());

                // if token was not consumed, skip it
                // to prevent infinite loop in parser
                if (Current == startToken)
                {
                    GetNextToken();
                }
            }

            return members.ToImmutable();
        }

        private MemberSyntax ParseMember()
        {
            if (Current.Kind == SyntaxKind.FunctionKeyword)
                return ParseFunctionDeclaration();
            else
                return ParseGlobalStatement();
        }

        private MemberSyntax ParseFunctionDeclaration()
        {
            var func = MatchToken(SyntaxKind.FunctionKeyword);
            var identifier = MatchToken(SyntaxKind.IdentifierToken);
            var open = MatchToken(SyntaxKind.OpenParenthesisToken);
            var parameters = ParseParameters();
            var close = MatchToken(SyntaxKind.CloseParenthesisToken);
            var typeClause = ParseOptionalTypeClause();
            var body = ParseBlockStatement();
            return new FunctionDeclarationSyntax(func, identifier, open, parameters, close, typeClause, body);
        }

        private MemberSyntax ParseGlobalStatement()
        {
            var statement = ParseStatement();
            return new GlobalStatementSyntax(statement);
        }

        private StatementSyntax ParseStatement()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenBraceToken:
                    return ParseBlockStatement();
                case SyntaxKind.LetKeyword:
                case SyntaxKind.VarKeyword:
                    return ParseVariableDeclaration();
                case SyntaxKind.IfKeyword:
                    return ParseIfStatement();
                case SyntaxKind.ForKeyword:
                    return ParseForStatement();
                case SyntaxKind.DoKeyword:
                    return ParseDoWhileStatement();
                case SyntaxKind.WhileKeyword:
                    return ParseWhileStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        private StatementSyntax ParseForStatement()
        {
            var keyword = MatchToken(SyntaxKind.ForKeyword);
            var identifier = MatchToken(SyntaxKind.IdentifierToken);
            var equals = MatchToken(SyntaxKind.EqualsToken);
            var lowerBound = ParseExpression();
            var to = MatchToken(SyntaxKind.ToKeyword);
            var upperBound = ParseExpression();
            var body = ParseStatement();

            return new ForStatementSyntax(keyword, identifier, equals, lowerBound, to, upperBound, body);
        }

        private StatementSyntax ParseDoWhileStatement()
        {
            var keyword = MatchToken(SyntaxKind.DoKeyword);
            var body = ParseStatement();
            var whileKeyword = MatchToken(SyntaxKind.WhileKeyword);
            var condition = ParseExpression();

            return new DoWhileStatementSyntax(keyword, body, whileKeyword, condition);
        }

        private StatementSyntax ParseWhileStatement()
        {
            var keyword = MatchToken(SyntaxKind.WhileKeyword);
            var condition = ParseExpression();
            var statement = ParseStatement();

            return new WhileStatementSyntax(keyword, condition, statement);
        }

        private StatementSyntax ParseIfStatement()
        {
            var keyword = MatchToken(SyntaxKind.IfKeyword);
            var condition = ParseExpression();
            var statement = ParseStatement();
            var optionalElse = ParseOptionalElseClause();

            return new IfStatementSyntax(keyword, condition, statement, optionalElse);
        }

        private ElseClauseSyntax ParseOptionalElseClause()
        {
            if (Current.Kind != SyntaxKind.ElseKeyword)
                return null;

            var keyword = MatchToken(SyntaxKind.ElseKeyword);
            var statement = ParseStatement();
            return new ElseClauseSyntax(keyword, statement);
        }

        private StatementSyntax ParseVariableDeclaration()
        {
            var expected = Current.Kind == SyntaxKind.LetKeyword ?
                SyntaxKind.LetKeyword :
                SyntaxKind.VarKeyword;

            var keyword = MatchToken(expected);
            var identifier = MatchToken(SyntaxKind.IdentifierToken);
            var typeClause = ParseOptionalTypeClause();
            var equals = MatchToken(SyntaxKind.EqualsToken);
            var initializer = ParseExpression();

            return new VariableDeclarationSyntax(keyword, identifier, typeClause, equals, initializer);
        }

        private TypeClauseSyntax ParseOptionalTypeClause()
        {
            if (Current.Kind != SyntaxKind.ColonToken)
                return null;

            return ParseTypeClause();
        }

        private TypeClauseSyntax ParseTypeClause()
        {
            var colon = MatchToken(SyntaxKind.ColonToken);
            var identifier = MatchToken(SyntaxKind.IdentifierToken);

            return new TypeClauseSyntax(colon, identifier);
        }

        private StatementSyntax ParseBlockStatement()
        {
            var open = MatchToken(SyntaxKind.OpenBraceToken);
            var statements = ImmutableArray.CreateBuilder<StatementSyntax>();

            while (Current.Kind != SyntaxKind.EndOfFileToken &&
                   Current.Kind != SyntaxKind.CloseBraceToken)
            {
                var startToken = Current;

                statements.Add(ParseStatement());

                // if token was not consumed, skip it
                // to prevent infinite loop in parser
                if (Current == startToken)
                {
                    GetNextToken();
                }
            }

            var close = MatchToken(SyntaxKind.CloseBraceToken);

            return new BlockStatementSyntax(open, statements.ToImmutable(), close);
        }

        private StatementSyntax ParseExpressionStatement()
        {
            var exp = ParseExpression();
            return new ExpressionStatementSyntax(exp);
        }

        private ExpressionSyntax ParseExpression()
        {
            return ParseAssignmentExpression();
        }

        private ExpressionSyntax ParseAssignmentExpression()
        {
            if (Peek(0).Kind == SyntaxKind.IdentifierToken &&
                Peek(1).Kind == SyntaxKind.EqualsToken)
            {
                var id = GetNextToken();
                var eq = GetNextToken();
                var right = ParseAssignmentExpression();
                return new AssignmentExpressionSyntax(id, eq, right);
            }

            return ParseBinaryExpression();
        }

        private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
        {
            ExpressionSyntax left;
            var unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();

            if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
            {
                var operatorToken = GetNextToken();
                var operand = ParseBinaryExpression(unaryOperatorPrecedence);
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }
            while (true)
            {
                var precedence = Current.Kind.GetBinaryOperatorPrecedence();
                if (precedence == 0 || precedence <= parentPrecedence)
                    break;

                var operatorToken = GetNextToken();
                var right = ParseBinaryExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }
            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
            => Current.Kind switch
            {
                SyntaxKind.NumberToken => ParseNumberLiteral(),
                SyntaxKind.StringToken => ParseStringLiteral(),
                SyntaxKind.OpenParenthesisToken => ParenthesizedExpression(),
                var boolean when
                    boolean == SyntaxKind.TrueKeyword ||
                    boolean == SyntaxKind.FalseKeyword
                => ParseBooleanLiteral(),
                _ => ParseNameOrCallExpression()
            };

        private ExpressionSyntax ParseNumberLiteral()
        {
            var numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }

        private ExpressionSyntax ParseStringLiteral()
        {
            var stringToken = MatchToken(SyntaxKind.StringToken);
            return new LiteralExpressionSyntax(stringToken);
        }

        private ExpressionSyntax ParenthesizedExpression()
        {
            var left = MatchToken(SyntaxKind.OpenParenthesisToken);
            var expression = ParseExpression();
            var right = MatchToken(SyntaxKind.CloseParenthesisToken);
            return new ParenthesizedExpressionSyntax(left, expression, right);
        }

        private ExpressionSyntax ParseBooleanLiteral()
        {
            var isTrue = Current.Kind == SyntaxKind.TrueKeyword;
            var token = isTrue ? MatchToken(SyntaxKind.TrueKeyword) : MatchToken(SyntaxKind.FalseKeyword);
            return new LiteralExpressionSyntax(token, isTrue);
        }

        private ExpressionSyntax ParseNameOrCallExpression()
        {
            if (Peek(0).Kind == SyntaxKind.IdentifierToken &&
                Peek(1).Kind == SyntaxKind.OpenParenthesisToken)
                return ParseCallExpression();

            return ParseNameExpression();
        }

        private ExpressionSyntax ParseCallExpression()
        {
            var identifier = MatchToken(SyntaxKind.IdentifierToken);
            var open = MatchToken(SyntaxKind.OpenParenthesisToken);
            var args = ParseArguments();
            var close = MatchToken(SyntaxKind.CloseParenthesisToken);
            return new CallExpressionSyntax(identifier, open, args, close);
        }

        private SeparatedSyntaxList<ExpressionSyntax> ParseArguments()
        {
            var nodesAndSeparators = ImmutableArray.CreateBuilder<SyntaxNode>();
            while (Current.Kind != SyntaxKind.EndOfFileToken &&
                   Current.Kind != SyntaxKind.CloseParenthesisToken)
            {
                var expression = ParseExpression();
                nodesAndSeparators.Add(expression);

                if (Current.Kind != SyntaxKind.CloseParenthesisToken)
                {
                    var comma = MatchToken(SyntaxKind.CommaToken);
                    nodesAndSeparators.Add(comma);
                }
            }
            return new SeparatedSyntaxList<ExpressionSyntax>(nodesAndSeparators.ToImmutable());
        }

        private SeparatedSyntaxList<ParameterSyntax> ParseParameters()
        {
            var nodesAndSeparators = ImmutableArray.CreateBuilder<SyntaxNode>();
            while (Current.Kind != SyntaxKind.EndOfFileToken &&
                   Current.Kind != SyntaxKind.CloseParenthesisToken)
            {
                var p = ParseParameter();
                nodesAndSeparators.Add(p);

                if (Current.Kind != SyntaxKind.CloseParenthesisToken)
                {
                    var comma = MatchToken(SyntaxKind.CommaToken);
                    nodesAndSeparators.Add(comma);
                }
            }
            return new SeparatedSyntaxList<ParameterSyntax>(nodesAndSeparators.ToImmutable());
        }

        private ParameterSyntax ParseParameter()
        {
            var identifier = MatchToken(SyntaxKind.IdentifierToken);
            var typeClause = ParseTypeClause();
            return new ParameterSyntax(identifier, typeClause);
        }

        private ExpressionSyntax ParseNameExpression()
        {
            var token = MatchToken(SyntaxKind.IdentifierToken);
            return new NameExpressionSyntax(token);
        }
    }
}
