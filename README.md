# Word Finder

Class library that finds words in a character matrix and returns the top 10.

One string is one row. Every row has the same length. Neither side exceeds 64. Words are found left to right, or top to bottom. `Find` takes a word stream that can be large. It returns the words from that stream that occur in the matrix, at most 10, highest matrix count first. If none occur, it returns an empty sequence, not null.

There is no host and no `Main`. Build and test the library.

## Build and test

Requires the .NET 8 SDK.

```powershell
dotnet test WordFinder.sln
```

That restores, builds `src/WordFinder`, and runs the xUnit project.

## Public type

```csharp
namespace WordFinder;

public class WordFinder
{
    public WordFinder(IEnumerable<string> matrix) { }
    public IEnumerable<string> Find(IEnumerable<string> wordstream) { }
}
```

The namespace and the class share a name. A caller writes `new WordFinder.WordFinder(...)` or a using alias.

```csharp
using Finder = WordFinder.WordFinder;

string[] matrix =
[
    "abcdc",
    "fgwio",
    "chill",
    "pqnsd",
    "uvdxy",
];

Finder finder = new(matrix);
IEnumerable<string> found = finder.Find(["cold", "wind", "snow", "chill"]);
```

`found` is `chill`, `cold`, `wind`. The stream order above is the order in the challenge figure. It is not the result order.

| Word | Placements | Result |
| --- | --- | --- |
| `chill` | 1, horizontal, row 2 | yes |
| `cold` | 1, vertical, column 4, from row 0 | yes |
| `wind` | 1, vertical, column 2, from row 1 | yes |
| `snow` | 0 | no |

All three hits have count 1, so ordinal order decides: `chill`, `cold`, `wind`.

## Ranking

The brief asks for the top 10 most repeated words from the stream that are found in the matrix, and it says a word repeated inside the stream counts once. After that second rule, stream frequency cannot order anything: every distinct word has frequency 1. The repetition that remains is the number of placements in the matrix.

A placement is one start cell plus one direction. Overlaps count. A one-letter cell counts twice, once horizontal and once vertical. Right to left, bottom to top, and diagonals do not count.

`Find` keeps each stream word once. It drops words that are not in the matrix, and it does not store those misses. It sorts the hits by count descending, then by `StringComparer.Ordinal` ascending, and returns at most 10. The same order breaks a tie for 10th place. Comparison is case-sensitive. An empty string is skipped. The result is an array: a second enumeration does not read the stream again.

## Index

The constructor validates the matrix and builds `Dictionary<string, int>` with `StringComparer.Ordinal`. Every horizontal and vertical slice of length at least 1 increments its count. `Find` probes that dictionary. It does not walk the matrix.

The matrix is at most 64×64, so the index is bounded and independent of the stream. The stream is the large input.

Placements inserted:

- horizontal: `R * C * (C + 1) / 2`
- vertical: `C * R * (R + 1) / 2`

At 64×64 that is 133,120 per direction, 266,240 in total. That is an upper bound on dictionary keys, reached only if every placement string is unique.

Character copies while building the slices, per direction, for a square side `N`:

`N * N * (N + 1) * (N + 2) / 6`

At 64 that is 2,928,640 characters per direction, 5,857,280 for both. These are bounds derived from the formulas, not measurements. No elapsed time is claimed.

`Find` then sorts at most `H` hits, where `H` cannot exceed the number of distinct indexed strings. Absent stream words are not retained.

## Invalid input

| Input | Result |
| --- | --- |
| Null matrix, null row, jagged rows, or either side above 64 | `ArgumentException`, `ParamName` `matrix` |
| Zero rows, or a rectangular width of 0 | Valid. `Find` returns empty |
| Null stream | `ArgumentNullException`, `ParamName` `wordstream` |
| Null word in the stream | `ArgumentException`, `ParamName` `wordstream` |
| Empty stream, or only empty strings | Empty sequence |
