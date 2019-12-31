using CodeAnalysis.Binding.Nodes;
using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace CodeAnalysis.Binding
{
    internal sealed class ControlFlowGraph
    {
        public ControlFlowGraph(BasicBlock start, BasicBlock end, List<BasicBlock> blocks, List<BasicBlockBranch> branches)
        {
            Start = start;
            End = end;
            Blocks = blocks;
            Branches = branches;
        }

        public BasicBlock Start { get; }
        public BasicBlock End { get; }
        public List<BasicBlock> Blocks { get; }
        public List<BasicBlockBranch> Branches { get; }

        public void WriteTo(TextWriter writer)
        {
            string Quote(string text)
                => "\"" + text.Replace("\"", "\\\"") + "\"";

            writer.WriteLine("digraph G {");
            var blockIds = Blocks
                .Select((x, i) => (Key: x, Value: $"N{i}"))
                .ToDictionary(x => x.Key, x => x.Value);
            foreach (var block in Blocks)
            {
                var id = blockIds[block];
                var label = Quote(block.ToString().Replace(Environment.NewLine, "\\l"));
                writer.WriteLine($"    {id} [label = {label} shape = box]");
            }
            foreach (var branch in Branches)
            {
                var from = blockIds[branch.From];
                var to = blockIds[branch.To];
                var label = Quote(branch.ToString());
                writer.WriteLine($"    {from} -> {to} [label = {label}]");
            }
            writer.WriteLine("}");
        }

        public static ControlFlowGraph Create(BoundBlockStatement body)
        {
            var builder = new BasicBlockBuilder();
            var blocks = builder.Build(body);

            return new GraphBuilder().Build(blocks);
        }

        public static bool AllPathReturn(BoundBlockStatement body)
        {
            var graph = Create(body);
            foreach (var branch in graph.End.Incoming)
            {
                var last = branch.From.Statements.Last();
                if (last.Kind != BoundNodeKind.ReturnStatement)
                    return false;
            }
            return true;
        }
    }

    internal sealed class BasicBlockBuilder
    {
        private readonly List<BoundStatement> _statements = new List<BoundStatement>();
        private readonly List<BasicBlock> _blocks = new List<BasicBlock>();

        public List<BasicBlock> Build(BoundBlockStatement block)
        {
            foreach (var statement in block.Statements)
            {
                switch (statement.Kind)
                {
                    case BoundNodeKind.LabelStatement:
                        StartBlock();
                        _statements.Add(statement);
                        break;
                    case BoundNodeKind.GoToStatement:
                    case BoundNodeKind.ConditionalGoToStatement:
                    case BoundNodeKind.ReturnStatement:
                        _statements.Add(statement);
                        StartBlock();
                        break;
                    case BoundNodeKind.ExpressionStatement:
                    case BoundNodeKind.VariableDeclaration:
                        _statements.Add(statement);
                        break;
                    default:
                        throw new Exception($"Unexpected statement {statement.Kind}");
                }
            }
            EndBlock();
            return _blocks.ToList();
        }

        private void StartBlock()
        {
            EndBlock();
        }

        private void EndBlock()
        {
            if (_statements.Any())
            {
                var block = new BasicBlock();
                block.Statements.AddRange(_statements);
                _blocks.Add(block);
                _statements.Clear();
            }
        }
    }

    internal sealed class GraphBuilder
    {
        private readonly List<BasicBlockBranch> _branches = new List<BasicBlockBranch>();
        private readonly Dictionary<BoundStatement, BasicBlock> _blockFromStatement = new Dictionary<BoundStatement, BasicBlock>();
        private readonly Dictionary<BoundLabel, BasicBlock> _blockFromLabel = new Dictionary<BoundLabel, BasicBlock>();
        private readonly BasicBlock _start = new BasicBlock(true);
        private readonly BasicBlock _end = new BasicBlock(false);

        public ControlFlowGraph Build(List<BasicBlock> blocks)
        {
            if (!blocks.Any())
                Connect(_start, _end);
            else
                Connect(_start, blocks.First());

            foreach (var block in blocks)
                foreach (var s in block.Statements)
                {
                    _blockFromStatement.Add(s, block);
                    if (s is BoundLabelStatement l)
                        _blockFromLabel.Add(l.Label, block);
                }

            for (int i = 0; i < blocks.Count; i++)
            {
                var current = blocks[i];
                var next = i == blocks.Count - 1 ? _end : blocks[i + 1];
                foreach (var s in current.Statements)
                {
                    var isLastStatementInBlock = s == current.Statements.Last();
                    switch (s.Kind)
                    {
                        case BoundNodeKind.GoToStatement:
                            var gs = (BoundGoToStatement)s;
                            Connect(current, _blockFromLabel[gs.Label]);
                            break;
                        case BoundNodeKind.ConditionalGoToStatement:
                            var cgs = (BoundConditionalGoToStatement)s;
                            var thenBlock = _blockFromLabel[cgs.Label];
                            var elseBlock = next;
                            var negated = Negate(cgs.Condition);
                            var thenCondition = cgs.JumpIfFalse ? negated : cgs.Condition;
                            var elseCondition = cgs.JumpIfFalse ? cgs.Condition : negated;
                            Connect(current, thenBlock, thenCondition);
                            Connect(current, elseBlock, elseCondition);
                            break;
                        case BoundNodeKind.ReturnStatement:
                            Connect(current, _end);
                            break;
                        case BoundNodeKind.VariableDeclaration:
                        case BoundNodeKind.LabelStatement:
                        case BoundNodeKind.ExpressionStatement:
                            if (isLastStatementInBlock)
                                Connect(current, next);
                            break;
                        default:
                            throw new Exception($"Unsupported kind {s.Kind}");
                    }
                }
            }

            ScanAgain:
            foreach (var block in blocks)
            {
                if (!block.Incoming.Any())
                {
                    RemoveBlock(blocks, block);
                    goto ScanAgain;
                }
            }

            blocks.Insert(0, _start);
            blocks.Add(_end);

            return new ControlFlowGraph(_start, _end, blocks, new List<BasicBlockBranch>());
        }

        private void RemoveBlock(List<BasicBlock> blocks, BasicBlock block)
        {
            foreach (var branch in block.Incoming)
            {
                branch.From.Outgoing.Remove(branch);
                _branches.Remove(branch);
            }

            foreach (var branch in block.Outgoing)
            {
                branch.To.Incoming.Remove(branch);
                _branches.Remove(branch);
            }

            blocks.Remove(block);
        }

        private BoundExpression Negate(BoundExpression condition)
        {
            if (condition is BoundLiteralExpression l)
                return new BoundLiteralExpression(!(bool)l.Value);

            var unaryOp = BoundUnaryOperator.Bind(SyntaxKind.BangToken, TypeSymbol.Bool);
            return new BoundUnaryExpression(unaryOp, condition);
        }

        private void Connect(BasicBlock from, BasicBlock to, BoundExpression condition = null)
        {
            if (condition is BoundLiteralExpression l)
            {
                var value = (bool)l.Value;
                if (value)
                    condition = null;
                else
                    return;
            }
            var branch = new BasicBlockBranch(from, to, condition);
            from.Outgoing.Add(branch);
            to.Incoming.Add(branch);
            _branches.Add(branch);
        }
    }

    internal sealed class BasicBlock
    {
        public BasicBlock()
        {
        }

        public BasicBlock(bool isStart)
        {
            IsStart = isStart;
            IsEnd = !isStart;
        }

        public List<BoundStatement> Statements { get; } = new List<BoundStatement>();
        public List<BasicBlockBranch> Incoming { get; } = new List<BasicBlockBranch>();
        public List<BasicBlockBranch> Outgoing { get; } = new List<BasicBlockBranch>();
        public bool IsStart { get; }
        public bool IsEnd { get; }

        public override string ToString()
        {
            if (IsStart) return "<Start>";
            if (IsEnd) return "<End>";

            using (var writer = new StringWriter())
            {
                foreach (var s in Statements)
                    s.WriteTo(writer);

                return writer.ToString();
            }
        }
    }

    internal sealed class BasicBlockBranch
    {
        public BasicBlockBranch(BasicBlock from, BasicBlock to, BoundExpression condition)
        {
            From = from;
            To = to;
            Condition = condition;
        }

        public BasicBlock From { get; }
        public BasicBlock To { get; }
        public BoundExpression Condition { get; }

        public override string ToString()
        {
            if (Condition == null)
                return "";

            return Condition.ToString();
        }
    }
}
