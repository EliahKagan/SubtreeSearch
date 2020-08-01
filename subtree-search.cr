require "deque"

# A mutable node for a binary tree with no parent pointers.
class Node(T)
  property key : T
  property left : Node(T)?
  property right : Node(T)?

  def initialize(@key, @left, @right)
  end

  def initialize(@key)
    @left = @right = nil
  end
end

# Compactly expresses an internal node.
def tree(key : T, left : Node(T)?, right : Node(T)?) forall T
  Node(T).new(key, left, right)
end

# Compactly expresses a leaf node.
def tree(key : T) forall T
  Node(T).new(key)
end

# Serializes the tree rooted at *root* as DOT code, intepretable by Graphviz.
# Inspired by "Visualizing binary trees with Graphviz" by Eli Bendersky:
# https://eli.thegreenplace.net/2009/11/23/visualizing-binary-trees-with-graphviz
# That approach uses a node's key to identify a vertex. Here, I identify all
# vertices by ascending integers and separately label each with its node's key
# (for those not representing nil). This permits trees with duplicate keys.
def dot(root : Node(T)?, name = "Tree", io = STDOUT, indent = 4) forall T
  margin = " " * indent
  io.puts "digraph #{name} {"

  queue = Deque(Tuple(Node(T), Int32)).new # TODO: Maybe used named tuples.
  order = 0 # The number of vertices encountered so far.

  emit_vertex = ->(node : Node(T)?) do
    vertex = order
    order += 1
    if node
      io.puts %{#{margin}#{vertex} [label="#{node.key}"]}
      queue.push({node, vertex})
    else
      io.puts "#{margin}#{vertex} [shape=point]"
    end
    vertex
  end

  emit_edge = ->(src : Int32, dest : Int32) do
    io.puts "#{margin}#{src} -> #{dest}"
    nil
  end

  emit_vertex.call(root)
  until queue.empty?
    parent_node, parent_vertex = queue.shift
    emit_edge.call(parent_vertex, emit_vertex.call(parent_node.left))
    emit_edge.call(parent_vertex, emit_vertex.call(parent_node.right))
  end

  io.puts "}"
end

pattern = tree("cat", tree("mule"), tree("horse"))
dot(pattern, "Pattern")
