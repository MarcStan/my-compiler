using CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace CodeAnalysis.Binding.Nodes
{
    internal sealed class BoundCallExpression : BoundExpression
    {
        public BoundCallExpression(
            FunctionSymbol function,
            ImmutableArray<BoundExpression> arguments)
        {
            Function = function;
            Arguments = arguments;
        }

        public FunctionSymbol Function { get; }
        public ImmutableArray<BoundExpression> Arguments { get; }

        public override TypeSymbol Type => Function.ReturnType;
        public override BoundNodeKind Kind => BoundNodeKind.CallExpression;
    }
}
