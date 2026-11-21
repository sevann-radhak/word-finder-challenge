namespace WordFinder;

public class WordFinder
{
    private const int MaxSide = 64;
    private readonly Dictionary<string, int> _counts;

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

            if (word.Length == 0 || hits.ContainsKey(word))
            {
                continue;
            }

            if (_counts.TryGetValue(word, out int count) && count > 0)
            {
                hits.Add(word, count);
            }
        }

        return hits.Keys.ToArray();
    }

    private static Dictionary<string, int> Index(List<string> rows)
    {
        Dictionary<string, int> counts = new(StringComparer.Ordinal);
        if (rows.Count == 0 || rows[0].Length == 0)
        {
            return counts;
        }

        int height = rows.Count;
        int width = rows[0].Length;

        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                for (int length = 1; length <= width - column; length++)
                {
                    Add(counts, rows[row].Substring(column, length));
                }
            }
        }

        for (int column = 0; column < width; column++)
        {
            for (int row = 0; row < height; row++)
            {
                for (int length = 1; length <= height - row; length++)
                {
                    Add(counts, ReadVertical(rows, row, column, length));
                }
            }
        }

        return counts;
    }

    private static string ReadVertical(List<string> rows, int startRow, int column, int length)
    {
        char[] buffer = new char[length];
        for (int i = 0; i < length; i++)
        {
            buffer[i] = rows[startRow + i][column];
        }

        return new string(buffer);
    }

    private static void Add(Dictionary<string, int> counts, string slice)
    {
        _ = counts.TryGetValue(slice, out int count);
        counts[slice] = count + 1;
    }
}
