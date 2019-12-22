using System.Collections.Generic;

namespace CodeAnalysis
{
    public sealed class EvaluationResult
    {
        public EvaluationResult(IReadOnlyList<string> diagnostics, object value)
        {
            Diagnostics = diagnostics;
            Value = value;
        }

        public IReadOnlyList<string> Diagnostics { get; }

        public object Value { get; }
    }
}
