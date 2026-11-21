using Finder = WordFinder.WordFinder;

namespace WordFinder.Tests;

public class WordFinderTests
{
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
}
