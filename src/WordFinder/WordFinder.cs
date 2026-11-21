namespace WordFinder;

public class WordFinder
{
    private const int _mAX_SIDE = 64;
    private readonly string[] _rows;

    public WordFinder(IEnumerable<string> matrix)
    {
        if (matrix is null)
        {
            throw new ArgumentException("Matrix is required.", nameof(matrix));
        }

        List<string> rows = new(_mAX_SIDE);
        int? width = null;

        foreach (var row in matrix)
        {
            if (row is null)
            {
                throw new ArgumentException("A matrix row is null.", nameof(matrix));
            }

            if (rows.Count == _mAX_SIDE)
            {
                throw new ArgumentException("Matrix side cannot exceed 64.", nameof(matrix));
            }

            if (row.Length > _mAX_SIDE)
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

        _rows = [.. rows];
    }

    public IEnumerable<string> Find(IEnumerable<string> wordstream)
    {
        ArgumentNullException.ThrowIfNull(wordstream);

        foreach (var word in wordstream)
        {
            if (word is null)
            {
                throw new ArgumentException("A word in the stream is null.", nameof(wordstream));
            }
        }

        return _rows.Length == 0 ? [] : Array.Empty<string>();
    }
}
