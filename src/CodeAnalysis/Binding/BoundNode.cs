using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CodeAnalysis.Binding
{
    internal abstract class BoundNode
    {
        public abstract BoundNodeKind Kind { get; }

        public IEnumerable<BoundNode> GetChildren()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = (BoundNode)property.GetValue(this);
                    if (child != null)
                        yield return child;
                }
                else if (typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = (IEnumerable<BoundNode>)property.GetValue(this);
                    foreach (var child in children)
                        if (child != null)
                            yield return child;
                }
            }
        }

        public IEnumerable<(string name, object value)> GetProperties()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.Name == nameof(Kind) ||
                    property.Name == nameof(BoundBinaryExpression.Operator) ||
                    property.Name == nameof(BoundUnaryExpression.Operator))
                    continue;

                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType) ||
                    typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                    continue;

                var value = property.GetValue(this);
                if (value != null)
                    yield return (property.Name, value);
            }
        }

        public void WriteTo(TextWriter writer)
            => PrintTree(writer, this);

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }

        private static void PrintTree(TextWriter writer, BoundNode node, string indent = "", bool isLast = true)
        {
            var marker = isLast ? "└─" : "├─";
            writer.Write(indent);
            writer.Write(marker);

            writer.Write(GetText(node));
            var isFirstProperty = true;

            foreach (var p in node.GetProperties())
            {
                if (isFirstProperty)
                {
                    isFirstProperty = false;
                    writer.Write(": ");
                }
                else
                    writer.Write(", ");

                writer.Write(p.name);
                writer.Write(" = ");
                writer.Write(p.value);
            }
            writer.WriteLine();

            indent += isLast ? "  " : "│ ";

            var last = node.GetChildren().LastOrDefault();

            foreach (var c in node.GetChildren())
                PrintTree(writer, c, indent, c == last);
        }

        private static string GetText(BoundNode node)
        {
            if (node is BoundUnaryExpression u)
                return u.Operator.Kind.ToString() + "Expression";
            if (node is BoundBinaryExpression b)
                return b.Operator.Kind.ToString() + "Expression";

            return node.Kind.ToString() + "Expression";
        }
    }
}
