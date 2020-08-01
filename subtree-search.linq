<Query Kind="Program" />

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
        var matches = new List<Node<T>>();
        
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
        matches.Dump(nameof(matches));
        matches[0].Right!.Left = Tree("donkey");
        
        tree.Dump($"{nameof(tree)} (after modification)", depth: 10);
    }
}
