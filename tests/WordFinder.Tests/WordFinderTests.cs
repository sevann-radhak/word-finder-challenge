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

    [Fact]
    public void FindReturnsChillColdAndWindFromTheSample()
    {
        Finder finder = new(Sample);
        IEnumerable<string> result = finder.Find(["cold", "wind", "snow", "chill"]);

        AssertSequence(result, "chill", "cold", "wind");
    }

    [Fact]
    public void FindRanksByMatrixCount()
    {
        Finder finder = new(["aaa"]);
        IEnumerable<string> result = finder.Find(["aaa", "aa", "a"]);

        AssertSequence(result, "a", "aa", "aaa");
    }

    [Fact]
    public void FindCountsBothDirectionsFromTheSameCell()
    {
        Finder finder = new(["cold", "oxxx", "lxxx", "dxxy"]);
        IEnumerable<string> result = finder.Find(["cold", "oxxx"]);

        AssertSequence(result, "cold", "oxxx");
    }

    [Fact]
    public void FindCountsASingleLetterTwice()
    {
        Finder finder = new(["a"]);
        IEnumerable<string> result = finder.Find(["a"]);

        AssertSequence(result, "a");
    }

    [Fact]
    public void FindOrdersTiesByOrdinal()
    {
        Finder finder = new(["ab"]);
        IEnumerable<string> result = finder.Find(["b", "a"]);

        AssertSequence(result, "a", "b");
    }

    [Fact]
    public void FindReturnsAtMostTenWords()
    {
        Finder finder = new(["abcdefghijk"]);
        IEnumerable<string> result = finder.Find(["k", "j", "i", "h", "g", "f", "e", "d", "c", "b", "a"]);

        AssertSequence(result, "a", "b", "c", "d", "e", "f", "g", "h", "i", "j");
    }

    [Fact]
    public void FindUsesOrdinalToBreakTheTenthPlace()
    {
        Finder finder = new(["acacacadadadaeaeaeafafafagagagahahahaiaiaiajajajakakakbm"]);
        IEnumerable<string> result = finder.Find(["m", "b", "ak", "aj", "ai", "ah", "ag", "af", "ae", "ad", "ac"]);

        AssertSequence(result, "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "b");
    }

    [Fact]
    public void FindCountsARepeatedStreamWordOnce()
    {
        Finder finder = new(["cold", "oxxx", "lxxx", "dxxy"]);
        IEnumerable<string> result = finder.Find(["oxxx", "oxxx", "oxxx", "cold"]);

        AssertSequence(result, "cold", "oxxx");
    }

    [Fact]
    public void FindReturnsEmptyWhenNothingIsFound()
    {
        Finder finder = new(Sample);
        IEnumerable<string> result = finder.Find(["snow"]);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void FindIgnoresEmptyWordsAmongRealWords()
    {
        Finder finder = new(Sample);
        IEnumerable<string> result = finder.Find(["", "cold", ""]);

        AssertSequence(result, "cold");
    }

    [Fact]
    public void FindCanBeEnumeratedTwiceWithoutReadingTheStreamAgain()
    {
        Finder finder = new(Sample);
        bool read = false;
        IEnumerable<string> result = finder.Find(ReadOnce());
        string[] first = result.ToArray();
        string[] second = result.ToArray();

        Assert.Equal(first, second);
        AssertSequence(first, "chill", "cold", "wind");

        IEnumerable<string> ReadOnce()
        {
            if (read)
            {
                throw new InvalidOperationException("The stream was read twice.");
            }

            yield return "cold";
            yield return "wind";
            yield return "snow";
            yield return "chill";
        }
    }

    private static void AssertSequence(IEnumerable<string> actual, params string[] expected)
    {
        Assert.Equal(expected, actual.ToArray());
    }

    private static void AssertSameWords(IEnumerable<string> actual, params string[] expected)
    {
        string[] actualSorted = actual.OrderBy(word => word, StringComparer.Ordinal).ToArray();
        string[] expectedSorted = expected.OrderBy(word => word, StringComparer.Ordinal).ToArray();
        Assert.Equal(expectedSorted, actualSorted);
    }
}
