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

# SubtreeSearch

These programs demonstrate linear-time subtree search:

- in Crystal (emitting DOT code), *and*
- in C# (emitting a LINQPad dump graph, including DOT code)

## License

This repository is licensed under [0BSD](https://spdx.org/licenses/0BSD.html).
See [**`LICENSE`**](LICENSE).

## How it works

The algorithm does postorder traversal of the &ldquo;corpus&rdquo; tree to
search in, memoizing every unique subtree in a hash table. The values of the
subtree area sequential subtree IDs. Each keys is a subroot element
(&ldquo;key&rdquo;), the ID representing its left subtree, and the ID
representing its right subtree. When a child pointer is `nil`/`null`, the ID is
0. Nonemty subtrees are given strictly positive IDs, starting with 1.

Then it does the same thing for the &ldquo;pattern&rdquo; tree to be searched,
except no new mappings in the hash table are created: if any subtree of the
pattern tree, including the entire pattern tree, is not already memoized, then
then the corpus tree does not contain (any copy of) the pattern tree. Otherwise
it does.

## History

An earlier version of this repository exists [as a
gist](https://gist.github.com/EliahKagan/61bce6756440f13cd5e1465538c18107). This
repository supersedes that gist. Currently the main difference, besides the
presence of this README file, is that the C#/LINQPad version in the Gist was
never updated to include DOT code generation. (The Crystal version does generate
DOT code.)

SubtreeSearch was written in 2020. This README was added in 2021.
