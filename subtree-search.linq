<Query Kind="Program" />

// Linear time subtree search
// (in C#, for LINQPad, with GraphViz DOT generation)
//
// Copyright (c) 2020 Eliah Kagan <degeneracypressure@gmail.com>
//
// Permission to use, copy, modify, and/or distribute this software for any
// purpose with or without fee is hereby granted.
//
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
// WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
// MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY
// SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
// WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION
// OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN
// CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

// Paste this code into LINQPad (https://www.linqpad.net/).

/// <summary>
/// A mutable node for a binary tree with no parent pointers.
/// </summary>
internal sealed class Node<T> {
    public Node(T key, Node<T>? left, Node<T>? right)
        => (Key, Left, Right) = (key, left, right);
    
    public Node(T key) : this(key, null, null) { }

    public T Key { get; set; }
    public Node<T>? Left { get; set; }
    public Node<T>? Right { get; set; }
}

internal static class NodeExtensions {
    /// <summary>
    /// Finds all subtrees in <c>self</c> that match <c>other</c>.
    /// </summary>
    /// <remarks>
    /// Runtime is linear in the sum of the sizes of the two trees.
    /// </remarks>
    internal static List<Node<T>> FindAll<T>(this Node<T>? self, Node<T> other)
    {
        var codes = new Dictionary<(T, int, int), int>(); // (key, code, code) -> code
        
        int Encode(Node<T>? node)
        {
            if (node == null) return 0;
            
            var triple = (node.Key, Encode(node.Left), Encode(node.Right));
            
            if (!codes.TryGetValue(triple, out var code)) {
                code = codes.Count + 1; // +1 because null -> 0 implicitly
                codes.Add(triple, code);
            }
            
            return code;
        }
        
        var patternCode = Encode(other);
        var matches = new List<Node<T>>();
        
        int? Search(Node<T>? node)
        {
            if (node == null) return 0;
            
            var leftCode = Search(node.Left);
            var rightCode = Search(node.Right);
            if (leftCode == null || rightCode == null) return null;
            
            var triple = (node.Key, (int)leftCode, (int)rightCode);
            if (!codes.TryGetValue(triple, out var code)) return null;
            if (code == patternCode) matches.Add(node);
            return code;
        }
        
        Search(self);
        return matches;
    }
    
    /// <summary>
    /// Serializes the tree rooted at <c>root</c> as DOT code, interpretable by
    /// Graphviz.
    /// </summary>
    /// <remarks>
    /// Inspired by "Visualizing binary trees with Graphviz" by Eli Bendersky:
    /// https://eli.thegreenplace.net/2009/11/23/visualizing-binary-trees-with-graphviz
    /// That approach uses a node's key to identify a vertex. Here, I identify
    /// all vertices by ascending integers and separately label each with its
    /// node's key (for those not representing null). This permits trees with
    /// duplicate keys.
    /// </remarks>
    internal static string
    ToDot<T>(this Node<T>? self, string name = "Tree", int indent = 4)
    {
        var margin = new string(' ', indent);
        var builder = new StringBuilder();
        builder.AppendLine($@"digraph ""{name}"" {{");
        
        var queue = new Queue<(Node<T> node, int vertex)>();
        var order = 0; // The number of vertices encountered so far.
        
        int EmitVertex(Node<T>? node)
        {
            if (node == null) {
                builder.AppendLine($@"{margin}{order} [shape=point]");
            } else {
                builder.AppendLine($@"{margin}{order} [label=""{node.Key}""]");
                queue.Enqueue((node: node, vertex: order));
            }
            return order++;
        }
        
        void EmitEdge(int src, int dest)
            => builder.AppendLine($"{margin}{src} -> {dest}");
        
        for (EmitVertex(self); queue.Count != 0; ) {
            var parent = queue.Dequeue();
            EmitEdge(parent.vertex, EmitVertex(parent.node.Left));
            EmitEdge(parent.vertex, EmitVertex(parent.node.Right));
        }
        
        builder.AppendLine("}");
        return builder.ToString();
    }
}

internal static class UnitTest {
    /// <summary>
    /// Helper function for readably expressing an internal node.
    /// </summary>
    private static Node<T> Tree<T>(T key, Node<T>? left, Node<T>? right)
        => new Node<T>(key, left, right);
    
    /// <summary>
    /// Helper function for readably expressing a leaf node.
    /// </summary>
    private static Node<T> Tree<T>(T key) => new Node<T>(key);
    
    private static void Main()
    {
        var tree = Tree("dog", Tree("cat", Tree("mule"),
                                           Tree("horse")),
                               Tree("snake", Tree("lizard", null,
                                                            Tree("iguana")),
                                             Tree("fox", Tree("cat", Tree("mule"),
                                                                     Tree("horse")),
                                                         Tree("human"))));
        
        var pattern = Tree("cat", Tree("mule"), Tree("horse"));
        
        var matches = tree.FindAll(pattern);
        matches.Select((Match, index) => new {
                            Match,
                            DOT = Match.ToDot($"Match {index}")
                        })
                .Dump("Matches");
        
        matches[0].Right!.Left = Tree("donkey");
        tree.Dump($"Tree after modification", depth: 10);
        tree.ToDot("Tree after modification")
            .Dump("Tree after modification (DOT)");
    }
}
