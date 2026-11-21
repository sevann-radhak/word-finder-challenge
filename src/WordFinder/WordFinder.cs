namespace WordFinder;

public class WordFinder
{
    private const int MaxSide = 64;
    private readonly HashSet<string> _words;
    private readonly int _maxWordLength;

    public WordFinder(IEnumerable<string> matrix)
    {
        if (matrix is null)
        {
            throw new ArgumentException("Matrix is required.", nameof(matrix));
        }

        List<string> rows = new(MaxSide);
        int? width = null;

        foreach (string row in matrix)
        {
            if (row is null)
            {
                throw new ArgumentException("A matrix row is null.", nameof(matrix));
            }

            if (rows.Count == MaxSide)
            {
                throw new ArgumentException("Matrix side cannot exceed 64.", nameof(matrix));
            }

            if (row.Length > MaxSide)
            {
                throw new ArgumentException("Matrix side cannot exceed 64.", nameof(matrix));
            }

            width ??= row.Length;
            if (row.Length != width)
            {
                throw new ArgumentException("Matrix rows must have the same length.", nameof(matrix));
            }

            rows.Add(row);
        }

        _maxWordLength = rows.Count == 0 || rows[0].Length == 0
            ? 0
            : Math.Max(rows.Count, rows[0].Length);

        _words = Index(rows);
    }

    public IEnumerable<string> Find(IEnumerable<string> wordstream)
    {
        ArgumentNullException.ThrowIfNull(wordstream);
        Dictionary<string, int> hits = new(StringComparer.Ordinal);

        foreach (string word in wordstream)
        {
            if (word is null)
            {
                throw new ArgumentException("A word in the stream is null.", nameof(wordstream));
            }

            if (word.Length == 0 || word.Length > _maxWordLength || !_words.Contains(word))
            {
                continue;
            }

            _ = hits.TryGetValue(word, out int count);
            hits[word] = count + 1;
        }

        return hits
            .OrderByDescending(pair => pair.Value)
            .ThenBy(pair => pair.Key, StringComparer.Ordinal)
            .Take(10)
            .Select(pair => pair.Key)
            .ToArray();
    }

    private static HashSet<string> Index(List<string> rows)
    {
        HashSet<string> words = new(StringComparer.Ordinal);
        if (rows.Count == 0 || rows[0].Length == 0)
        {
            return words;
        }

        foreach (string row in rows)
        {
            AddSlices(words, row);
        }

        int width = rows[0].Length;
        for (int column = 0; column < width; column++)
        {
            AddSlices(words, Column(rows, column));
        }

        return words;
    }

    private static string Column(List<string> rows, int column)
    {
        return string.Create(rows.Count, (rows, column), static (span, state) =>
        {
            List<string> source = state.rows;
            int sourceColumn = state.column;

            for (int row = 0; row < span.Length; row++)
            {
                span[row] = source[row][sourceColumn];
            }
        });
    }

    private static void AddSlices(HashSet<string> words, string line)
    {
        for (int start = 0; start < line.Length; start++)
        {
            for (int length = 1; length <= line.Length - start; length++)
            {
                _ = words.Add(line.Substring(start, length));
            }
        }
    }
}
