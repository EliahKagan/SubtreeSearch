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
  io.puts %(digraph "#{name}" {)

  queue = Deque(NamedTuple(node: Node(T), vertex: Int32)).new
  order = 0 # The number of vertices encountered so far.

  emit_vertex = ->(node : Node(T)?) do
    vertex = order
    order += 1
    if node
      io.puts %(#{margin}#{vertex} [label="#{node.key}"])
      queue.push({node: node, vertex: vertex})
    else
      io.puts %(#{margin}#{vertex} [shape=point])
    end
    vertex
  end

  emit_edge = ->(src : Int32, dest : Int32) do
    io.puts "#{margin}#{src} -> #{dest}"
    nil
  end

  emit_vertex.call(root)
  until queue.empty?
    parent = queue.shift
    emit_edge.call(parent[:vertex], emit_vertex.call(parent[:node].left))
    emit_edge.call(parent[:vertex], emit_vertex.call(parent[:node].right))
  end

  io.puts "}"
end

# Finds all the subtrees in *corpus* that match *pattern*.
# Runtime is linear in the sum of the sizes of the two trees.
def search(corpus : Node(T)?, pattern : Node(T)) forall T
  codes = {} of Tuple(T, Int32, Int32) => Int32 # {key, code, code} => code

  encode = Proc(Node(T)?, Int32).new { raise "Bug: encode not reassigned" }
  encode = ->(node : Node(T)?) do
    return 0 unless node
    triple = {node.key, encode.call(node.left), encode.call(node.right)}
    codes[triple] ||= codes.size + 1 # +1 because nil => 0 implicitly.
  end

  pattern_code = encode.call(pattern)
  matches = [] of Node(T)

  search = Proc(Node(T)?, Int32?).new { raise "Bug: search not reassigned" }
  search = ->(node : Node(T)?) do
    return 0 unless node

    left_code = search.call(node.left)
    right_code = search.call(node.right)
    return nil unless left_code && right_code

    code = codes[{node.key, left_code, right_code}]?
    return nil unless code
    matches << node if code == pattern_code
    code
  end

  search.call(corpus)
  matches
end

corpus = tree("dog", tree("cat", tree("mule"),
                                 tree("horse")),
                     tree("snake", tree("lizard", nil,
                                                  tree("iguana")),
                                   tree("fox", tree("cat", tree("mule"),
                                                           tree("horse")),
                                               tree("human"))))

pattern = tree("cat", tree("mule"), tree("horse"))

matches = search(corpus, pattern)
matches.each_with_index do |match, index|
  dot(match, "Match #{index}")
  puts
end

matches[0].right.as(Node(String)).left = tree("donkey")
dot(corpus, "corpus (after modification)")
