namespace WordFinder;

public class WordFinder
{
    private const int MaxSide = 64;
    private readonly Dictionary<string, int> _counts;
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

            if (width is null)
            {
                width = row.Length;
            }
            else if (row.Length != width)
            {
                throw new ArgumentException("Matrix rows must have the same length.", nameof(matrix));
            }

            rows.Add(row);
        }

        _maxWordLength = rows.Count == 0 || rows[0].Length == 0
            ? 0
            : Math.Max(rows.Count, rows[0].Length);

        _counts = Index(rows);
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

            if (word.Length == 0 || word.Length > _maxWordLength || hits.ContainsKey(word))
            {
                continue;
            }

            if (_counts.TryGetValue(word, out int count) && count > 0)
            {
                hits.Add(word, count);
            }
        }

        return hits
            .OrderByDescending(pair => pair.Value)
            .ThenBy(pair => pair.Key, StringComparer.Ordinal)
            .Take(10)
            .Select(pair => pair.Key)
            .ToArray();
    }

    private static Dictionary<string, int> Index(List<string> rows)
    {
        Dictionary<string, int> counts = new(StringComparer.Ordinal);
        if (rows.Count == 0 || rows[0].Length == 0)
        {
            return counts;
        }

        foreach (string row in rows)
        {
            AddSlices(counts, row);
        }

        int width = rows[0].Length;
        for (int column = 0; column < width; column++)
        {
            AddSlices(counts, Column(rows, column));
        }

        return counts;
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

    private static void AddSlices(Dictionary<string, int> counts, string line)
    {
        for (int start = 0; start < line.Length; start++)
        {
            for (int length = 1; length <= line.Length - start; length++)
            {
                Add(counts, line.Substring(start, length));
            }
        }
    }

    private static void Add(Dictionary<string, int> counts, string slice)
    {
        _ = counts.TryGetValue(slice, out int count);
        counts[slice] = count + 1;
    }
}
