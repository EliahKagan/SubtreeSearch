<!--
  Copyright (c) 2021 Eliah Kagan <degeneracypressure@gmail.com>
  
  Permission to use, copy, modify, and/or distribute this software for any
  purpose with or without fee is hereby granted.
  
  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH
  REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY
  AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT,
  INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM
  LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR
  OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
  PERFORMANCE OF THIS SOFTWARE.
-->

# SubtreeSearch - linear-time subtree search

These programs demonstrate
[linear-time](https://en.wikipedia.org/wiki/Time_complexity#Linear_time) subtree
search:

- in [Crystal](https://crystal-lang.org/) (emitting DOT code), *and*
- in C# (emitting a [LINQPad](https://www.linqpad.net/) dump, including DOT
  code).

If you want to render the DOT code as a graph drawing, you can do so by running
the [Graphviz](https://graphviz.org/) `dot` command, or simply by pasting the
DOT code into [GraphvizOnline](https://dreampuf.github.io/GraphvizOnline/) by
[dreampuf](https://github.com/dreampuf/GraphvizOnline).

## License

This repository is licensed under [0BSD](https://spdx.org/licenses/0BSD.html).
See [**`LICENSE`**](LICENSE).

## How it works

The algorithm does postorder traversal of the &ldquo;corpus&rdquo; tree to
search in, memoizing every unique subtree in a hash table. The values stored in
the table are sequential subtree IDs. Each key is an aggregate of the subroot
element (the node &ldquo;key&rdquo;), the ID representing the left subtree, and
the ID representing its right subtree. When a child pointer is `nil`/`null`, the
ID is
0. Nonempty subtrees are given strictly positive IDs, starting with 1.

Then it does the same thing for the &ldquo;pattern&rdquo; tree to be searched,
except no new mappings in the hash table are created: if any subtree of the
pattern tree, including the entire pattern tree, is not already memoized, then
then the corpus tree does not contain (any copy of) the pattern tree. Otherwise
it does.

Assuming good hash distribution, this algorithm runs in *O(M + N)* time with
high probability<sup>**&dagger;**</sup>, when searching a tree of *M* nodes for
a subtree that is a copy of a tree with *N* nodes.

<sub><sup>**&dagger;**</sup> Because finding or inserting a key in a hash table
takes amortized *O(1)* time with high probability.</sub>

## Acknowledgements

The way I&rsquo;ve drawn binary trees using GraphViz is inspired by
[&ldquo;Visualizing binary trees with
Graphviz&rdquo;](https://eli.thegreenplace.net/2009/11/23/visualizing-binary-trees-with-graphviz)
by [Eli Bandersky](https://eli.thegreenplace.net/pages/about).

See the comments above the `dot` method in `subtree-search.cr` and the
`NodeExtensions.ToDot` method in `subtree-search.linq` for information on how
the approach I&rsquo;ve used is similar and different.

## History

An earlier version of this repository exists [as a
gist](https://gist.github.com/EliahKagan/61bce6756440f13cd5e1465538c18107). This
repository supersedes that gist. Currently the main difference, besides the
presence of this README file, is that the C#/LINQPad version in the Gist was
never updated to include DOT code generation. (The Crystal version does generate
DOT code.)

SubtreeSearch was written in 2020. This README was added in 2021.
