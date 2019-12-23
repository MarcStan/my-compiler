using CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeAnalysis.Tests.Helpers
{
    internal sealed class AnnotatedText
    {
        public AnnotatedText(string text, ImmutableArray<TextSpan> spans)
        {
            Text = text;
            Spans = spans;
        }

        public string Text { get; }
        public ImmutableArray<TextSpan> Spans { get; }

        public static AnnotatedText Parse(string text)
        {
            text = Unindent(text);

            var builder = new StringBuilder();
            var spanBuilder = ImmutableArray.CreateBuilder<TextSpan>();
            var stack = new Stack<int>();

            var position = 0;

            foreach (var c in text)
            {
                if (c == '[')
                    stack.Push(position);
                else if (c == ']')
                {
                    if (!stack.Any())
                        throw new ArgumentException("Too many ']' in text", nameof(text));

                    var start = stack.Pop();
                    var end = position;
                    spanBuilder.Add(TextSpan.FromBounds(start, end));
                }
                else
                {
                    position++;
                    builder.Append(c);
                }
            }
            if (stack.Any())
                throw new ArgumentException("Missing ']' in text", nameof(text));

            return new AnnotatedText(builder.ToString(), spanBuilder.ToImmutable());
        }

        public static string[] UnindentLines(string text)
        {
            var lines = new List<string>();

            using (var reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    lines.Add(line);
            }

            var minIndent = int.MaxValue;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Trim().Length == 0)
                {
                    lines[i] = "";
                    continue;
                }

                var indent = lines[i].Length - lines[i].TrimStart().Length;
                minIndent = Math.Min(minIndent, indent);
            }
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Length == 0)
                    continue;

                lines[i] = lines[i].Substring(minIndent);
            }

            while (lines.Count > 0 && lines[0].Length == 0)
                lines.RemoveAt(0);
            while (lines.Count > 0 && lines[lines.Count - 1].Length == 0)
                lines.RemoveAt(lines.Count - 1);

            return lines.ToArray();
        }

        private static string Unindent(string text)
        {
            var lines = UnindentLines(text);

            return string.Join(Environment.NewLine, lines);
        }
    }
}
