using FullTextSearchDemo.SearchEngine.Engine;
using FullTextSearchDemo.SearchEngine.Queries;
using FullTextSearchDemo.SearchEngine.Services;
using FullTextSearchDemo.SearchEngine.Tests.TestModels;

namespace FullTextSearchDemo.SearchEngine.Tests;

public class PrimitivePropertiesTests
{
    private SearchEngine<Element> _searchEngine = null!;

    [SetUp]
    public void Setup()
    {
        var elements = new List<Element>()
        {
            new(),
            new()
            {
                BooleanProperty = true,
                ByteProperty = 5,
                SByteProperty = -3,
                CharProperty = 'A',
                DecimalProperty = 123.45m,
                DoubleProperty = 3.14159265,
                SingleProperty = 2.71828f,
                Int32Property = 42,
                UInt32Property = 100,
                IntPtrProperty = 42,
                UIntPtrProperty = 100,
                Int64Property = 1234567890,
                UInt64Property = 9876543210,
                Int16Property = 32767,
                UInt16Property = 65535
            },
            new()
            {
                BooleanProperty = true,
                ByteProperty = 1,
                SByteProperty = 1,
                CharProperty = '1',
                DecimalProperty = 1.0m,
                DoubleProperty = 1.0,
                SingleProperty = 1.0f,
                Int32Property = 1,
                UInt32Property = 1,
                IntPtrProperty = 1,
                UIntPtrProperty = 1,
                Int64Property = 1,
                UInt64Property = 1,
                Int16Property = 1,
                UInt16Property = 1
            }
        };

        var configuration = new AllPrimitiveConfiguration();

        var documentWriter = new DocumentWriter<Element>(configuration);
        _searchEngine = new SearchEngine<Element>(new DocumentReader<Element>(configuration), documentWriter);
        _searchEngine.AddRange(elements);
    }

    [TearDown]
    public void TearDown()
    {
        _searchEngine.RemoveAll();
        _searchEngine.DisposeResources();
    }

    [Test]
    [TestCase("1")]
    [TestCase("true")]
    [TestCase("5")]
    [TestCase("-3")]
    [TestCase("A")]
    public void Search_ExactSearchWithCorrectTerm_ReturnsAllElements(string search)
    {
        var result = _searchEngine.Search(new AllFieldsSearchQuery
            { SearchTerm = search, Type = SearchType.ExactMatch }).Items.ToList();

        Assert.That(result, Has.Count.EqualTo(3));
    }

    [Test]
    [TestCase(SearchType.ExactMatch)]
    [TestCase(SearchType.FuzzyMatch)]
    [TestCase(SearchType.PrefixMatch)]
    public void Search_ExactSearchWithEmptyTerm_ReturnsAllPosts(SearchType searchType)
    {
        var result = _searchEngine.Search(new AllFieldsSearchQuery
            { Type = searchType }).Items.ToList();

        Assert.That(result, Has.Count.EqualTo(3));
    }

    [Test]
    [TestCase(0, 2, 2)]
    [TestCase(1, 2, 1)]
    [TestCase(2, 2, 0)]
    [TestCase(0, 1, 1)]
    [TestCase(0, 1, 1)]
    [TestCase(1, 1, 1)]
    [TestCase(2, 1, 1)]
    [TestCase(3, 1, 0)]
    public void Search_WithEmptyTermWithPagination_ReturnsTowPosts(int pageNumber, int pageSize, int expectedPosts)
    {
        var result = _searchEngine.Search(new AllFieldsSearchQuery
            { Type = SearchType.ExactMatch, PageNumber = pageNumber, PageSize = pageSize }).Items.ToList();

        Assert.That(result, Has.Count.EqualTo(expectedPosts));
    }
}