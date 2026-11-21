using Finder = WordFinder.WordFinder;

namespace WordFinder.Tests;

public class WordFinderTests
{
    private static readonly string[] Sample = ["abcdc", "fgwio", "chill", "pqnsd", "uvdxy"];

    [Fact]
    public void ConstructorRejectsNullMatrix()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new Finder(null!));

        Assert.Equal("matrix", exception.ParamName);
    }

    [Fact]
    public void ConstructorRejectsNullRow()
    {
        string[] matrix = ["ab", null!];
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new Finder(matrix));

        Assert.Equal("matrix", exception.ParamName);
    }

    [Fact]
    public void ConstructorRejectsJaggedMatrix()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new Finder(["ab", "a"]));

        Assert.Equal("matrix", exception.ParamName);
    }

    [Fact]
    public void ConstructorRejectsMoreThan64Rows()
    {
        IEnumerable<string> matrix = Enumerable.Repeat("a", 65);
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new Finder(matrix));

        Assert.Equal("matrix", exception.ParamName);
    }

    [Fact]
    public void ConstructorRejectsMoreThan64Columns()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new Finder([new string('a', 65)]));

        Assert.Equal("matrix", exception.ParamName);
    }

    [Fact]
    public void ConstructorAccepts64By64()
    {
        string row = new('a', 64);
        IEnumerable<string> matrix = Enumerable.Repeat(row, 64);
        Finder finder = new(matrix);

        Assert.NotNull(finder);
    }

    [Fact]
    public void ConstructorAcceptsEmptyMatrix()
    {
        Finder finder = new([]);
        Assert.NotNull(finder);
    }

    [Fact]
    public void ConstructorAcceptsRowsOfLengthZero()
    {
        Finder finder = new(["", ""]);
        Assert.NotNull(finder);
    }

    [Fact]
    public void FindRejectsNullStream()
    {
        Finder finder = new(["a"]);
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => finder.Find(null!));
        Assert.Equal("wordstream", exception.ParamName);
    }

    [Fact]
    public void FindRejectsNullWord()
    {
        Finder finder = new(["a"]);
        ArgumentException exception = Assert.Throws<ArgumentException>(() => finder.Find([null!]));
        Assert.Equal("wordstream", exception.ParamName);
    }

    [Fact]
    public void FindReturnsEmptyForEmptyStream()
    {
        Finder finder = new(["a"]);
        IEnumerable<string> result = finder.Find([]);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void FindReturnsEmptyForEmptyMatrix()
    {
        Finder finder = new([]);
        IEnumerable<string> result = finder.Find(["cold"]);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void FindIgnoresEmptyWords()
    {
        Finder finder = new(["a"]);
        IEnumerable<string> result = finder.Find([""]);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void FindReturnsTheSampleWordsThatArePresent()
    {
        Finder finder = new(Sample);
        IEnumerable<string> result = finder.Find(["cold", "wind", "snow", "chill"]);

        AssertSameWords(result, "chill", "cold", "wind");
    }

    [Fact]
    public void FindDoesNotReturnAReverseWord()
    {
        Finder finder = new(["cold"]);
        IEnumerable<string> result = finder.Find(["dloc", "cold"]);

        AssertSameWords(result, "cold");
    }

    [Fact]
    public void FindDoesNotReturnABottomToTopWord()
    {
        Finder finder = new(["d", "l", "o", "c"]);
        IEnumerable<string> result = finder.Find(["cold", "dloc"]);

        AssertSameWords(result, "dloc");
    }

    [Fact]
    public void FindDoesNotReturnADiagonalWord()
    {
        Finder finder = new(["ab", "de"]);
        IEnumerable<string> result = finder.Find(["ae", "ab"]);

        AssertSameWords(result, "ab");
    }

    [Fact]
    public void FindIsCaseSensitive()
    {
        Finder finder = new(Sample);
        IEnumerable<string> result = finder.Find(["Chill", "chill"]);

        AssertSameWords(result, "chill");
    }

    [Fact]
    public void FindReturnsHorizontalAndVerticalWords()
    {
        Finder finder = new(["ab", "ac"]);
        IEnumerable<string> result = finder.Find(["ab", "ac"]);

        AssertSameWords(result, "ab", "ac");
    }

    private static void AssertSameWords(IEnumerable<string> actual, params string[] expected)
    {
        string[] actualSorted = actual.OrderBy(word => word, StringComparer.Ordinal).ToArray();
        string[] expectedSorted = expected.OrderBy(word => word, StringComparer.Ordinal).ToArray();
        Assert.Equal(expectedSorted, actualSorted);
    }
}
