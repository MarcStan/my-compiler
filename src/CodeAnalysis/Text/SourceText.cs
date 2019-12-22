using System.Collections.Immutable;

namespace CodeAnalysis.Text
{
    public sealed class SourceText
    {
        private readonly string _text;

        private SourceText(string text)
        {
            _text = text;
            Lines = ParseLines(this, text);
        }

        public ImmutableArray<TextLine> Lines { get; }

        public int GetLineIndex(int position)
        {
            var lower = 0;
            var upper = Lines.Length - 1;
            while (lower <= upper)
            {
                var index = lower + (upper - lower) / 2;
                var start = Lines[index].Start;
                if (position == start)
                    return index;
                if (start <= position)
                {
                    lower = index + 1;
                }
                else
                {
                    upper = index - 1;
                }
            }

            return lower - 1;
        }

        public char this[int index] => _text[index];

        public int Length => _text.Length;

        public override string ToString()
            => _text;

        public string ToString(int start, int length)
            => _text.Substring(start, length);

        public string ToString(TextSpan span)
            => _text.Substring(span.Start, span.Length);

        public static SourceText From(string text)
            => new SourceText(text);

        private static ImmutableArray<TextLine> ParseLines(SourceText sourceText, string text)
        {
            var result = ImmutableArray.CreateBuilder<TextLine>();

            var position = 0;
            var lineStart = 0;
            while (position < text.Length)
            {
                var lineBreakWidth = GetLineBreakWidth(text, position);

                if (lineBreakWidth == 0)
                {
                    position++;
                }
                else
                {
                    AddLine(result, sourceText, position, lineStart, lineBreakWidth);
                    position += lineBreakWidth;
                    lineStart = position;
                }
            }
            if (position >= lineStart)
                AddLine(result, sourceText, position, lineStart, 0);

            return result.ToImmutable();
        }

        private static void AddLine(ImmutableArray<TextLine>.Builder result, SourceText sourceText, int position, int lineStart, int lineBreakWidth)
        {
            var lineLength = position - lineStart;
            result.Add(new TextLine(sourceText, lineStart, lineLength, lineLength + lineBreakWidth));
        }

        private static int GetLineBreakWidth(string text, int i)
        {
            var c = text[i];
            var l = i + 1 >= text.Length ? '\0' : text[i + 1];
            if (c == '\r' && l == '\n')
                return 2;
            if (c == '\r' || c == '\n')
                return 1;

            return 0;
        }
    }
}
