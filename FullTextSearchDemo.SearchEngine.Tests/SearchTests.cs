using FullTextSearchDemo.SearchEngine.Engine;
using FullTextSearchDemo.SearchEngine.Queries;
using FullTextSearchDemo.SearchEngine.Services;
using FullTextSearchDemo.SearchEngine.Tests.TestModels;

namespace FullTextSearchDemo.SearchEngine.Tests;

public class SearchTests
{
    private SearchEngine<Post> _searchEngine = null!;

    private const string Title = "Testing Apache Lucene.NET - Ensuring robust search functionality in C#";

    [SetUp]
    public void Setup()
    {
        var postList = new List<Post>()
        {
            new()
            {
                Id = 1,
                Title = Title,
                Content = "<h1>Fox</h1>"
            },
            new()
            {
                Id = 2,
                Title = "Just another post about Search Engines",
                Content = "<h1>Solr rocks!</h1>"
            },

            new()
            {
                Id = 2,
                Title = "Search is cool with Apache Lucene.NET",
                Content = "<h1>Apache Lucene at the core!</h1>"
            }
        };

        var configuration = new PostTestConfiguration();
        _searchEngine = new SearchEngine<Post>(new DocumentReader<Post>(configuration),
            new DocumentWriter<Post>(configuration));

        _searchEngine.AddRange(postList);
    }

    [TearDown]
    public void TearDown()
    {
        _searchEngine.RemoveAll();
        _searchEngine.DisposeResources();
    }

    [Test]
    public void Remove_ExistingDocument_DocumentIsRemoved()
    {
        var toDelete = new Post
        {
            Id = 1,
            Title = Title,
            Content = "<h1>Fox</h1>"
        };

        _searchEngine.Remove(toDelete);

        var query = new AllFieldsSearchQuery { SearchTerm = "fox", Type = SearchType.ExactMatch };

        var result = _searchEngine.Search(query).Items.ToList();

        Assert.That(result, Has.Count.EqualTo(0));
    }

    [Test]
    public void RemoveAll_WithDocuments_IndexIsEmpty()
    {
        _searchEngine.RemoveAll();

        var result = _searchEngine.Search(new AllFieldsSearchQuery());

        Assert.Multiple(() =>
        {
            Assert.That(result.Items.Count(), Is.EqualTo(0));
            Assert.That(result.TotalItems, Is.EqualTo(0));
        });
    }

    [Test]
    public void Add_WithNoDocuments_DocumentIsAdded()
    {
        _searchEngine.RemoveAll();

        _searchEngine.Add(new Post
        {
            Id = 1,
            Title = "Updated title",
            Content = "<h1>Fox</h1>"
        });
        
        var result = _searchEngine.Search(new AllFieldsSearchQuery());

        Assert.Multiple(() =>
        {
            Assert.That(result.Items.Count(), Is.EqualTo(1));
            Assert.That(result.TotalItems, Is.EqualTo(1));
        });
    }

    [Test]
    public void Update_ExistingDocument_DocumentIsUpdated()
    {
        var toUpdate = new Post
        {
            Id = 1,
            Title = "Updated title",
            Content = "<h1>Fox</h1>"
        };

        _searchEngine.Update(toUpdate);

        var query = new AllFieldsSearchQuery { SearchTerm = "fox", Type = SearchType.ExactMatch };

        var result = _searchEngine.Search(query).Items.ToList();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("Updated title"));
            Assert.That(result[0].Content, Is.EqualTo("<h1>Fox</h1>"));
        });
    }

    [Test]
    public void Search_AllFieldQueryExactSearchWithCorrectTerm_ReturnsPost()
    {
        var query = new AllFieldsSearchQuery { SearchTerm = "fox", Type = SearchType.ExactMatch };

        var result = _searchEngine.Search(query).Items.ToList();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].UniqueKey, Is.EqualTo("1"));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo(Title));
        });
    }

    [Test]
    public void Search_AllFieldQueryExactSearchWithWrongTerm_ReturnsEmptyList()
    {
        var query = new AllFieldsSearchQuery { SearchTerm = "foy", Type = SearchType.ExactMatch };

        var result = _searchEngine.Search(query);

        Assert.That(result.Items.Count(), Is.EqualTo(0));
    }

    [Test]
    [TestCase(SearchType.ExactMatch)]
    [TestCase(SearchType.FuzzyMatch)]
    [TestCase(SearchType.PrefixMatch)]
    public void Search_AllFieldQueryExactSearchWithEmptyTerm_ReturnsAllPosts(SearchType searchType)
    {
        var query = new AllFieldsSearchQuery { Type = searchType };

        var result = _searchEngine.Search(query);

        Assert.That(result.Items.Count(), Is.EqualTo(3));
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
    public void Search_AllFieldQueryWithEmptyTermWithPagination_ReturnsTowPosts(int pageNumber, int pageSize,
        int expectedPosts)
    {
        var query = new AllFieldsSearchQuery
            { Type = SearchType.ExactMatch, PageNumber = pageNumber, PageSize = pageSize };

        var result = _searchEngine.Search(query);

        Assert.That(result.Items.Count(), Is.EqualTo(expectedPosts));
    }

    [Test]
    [TestCase("Testing", 1)]
    [TestCase("testIng", 1)]
    [TestCase("Search", 3)]
    [TestCase("Seorch", 3)]
    [TestCase("Apache", 2)]
    [TestCase("apache", 2)]
    [TestCase("fox", 0)]
    public void Search_OnlyTitleQueryFuzzySearchWithCorrectTerm_ReturnsAllPost(string search, int expected)
    {
        var query = new FieldSpecificSearchQuery
            { SearchTerms = new Dictionary<string, string?> { { "Title", search } }, Type = SearchType.FuzzyMatch };

        var result = _searchEngine.Search(query);

        Assert.That(result.Items.Count(), Is.EqualTo(expected));
    }

    [Test]
    [TestCase("Testing", 0)]
    [TestCase("testIng", 0)]
    [TestCase("testing", 1)]
    [TestCase("Search", 0)]
    [TestCase("search", 3)]
    [TestCase("seorch", 0)]
    [TestCase("Apache", 0)]
    [TestCase("apache", 2)]
    [TestCase("fox", 0)]
    [TestCase("Fox", 0)]
    public void Search_OnlyTitleQueryExactSearchWithCorrectTerm_ReturnsAllPost(string search, int expected)
    {
        var query = new FieldSpecificSearchQuery
            { SearchTerms = new Dictionary<string, string?> { { "Title", search } }, Type = SearchType.ExactMatch };

        var result = _searchEngine.Search(query);

        Assert.That(result.Items.Count(), Is.EqualTo(expected));
    }

    [Test]
    public void Search_AllFieldQueryWithPageSizeOne_HasThreePages()
    {
        var query = new AllFieldsSearchQuery { PageSize = 1 };

        var result = _searchEngine.Search(query);

        Assert.Multiple(() =>
        {
            Assert.That(result.Items.Count(), Is.EqualTo(1));
            Assert.That(result.TotalItems, Is.EqualTo(3));
            Assert.That(result.TotalPages, Is.EqualTo(3));
            Assert.That(result.PageNumber, Is.EqualTo(0));
        });
    }

    [Test]
    public void Search_AllFieldQueryWithPageSizeLessThan1_SetPageSizeTo10()
    {
        var query = new AllFieldsSearchQuery { PageSize = 0 };

        var result = _searchEngine.Search(query);

        Assert.Multiple(() =>
        {
            Assert.That(result.Items.Count(), Is.EqualTo(3));
            Assert.That(result.TotalItems, Is.EqualTo(3));
            Assert.That(result.TotalPages, Is.EqualTo(1));
            Assert.That(result.PageNumber, Is.EqualTo(0));
        });
    }

    [Test]
    [TestCase(5, 5)]
    [TestCase(-5, 0)]
    public void Search_AllFieldQueryWithInvalidPageNumber_PageNumberIsNotChanged(int pageNumber, int expectedPageNumber)
    {
        var query = new AllFieldsSearchQuery { PageNumber = pageNumber };

        var result = _searchEngine.Search(query);

        Assert.That(result.PageNumber, Is.EqualTo(expectedPageNumber));
    }

    [Test]
    public void Clear_IndexWithThreeDocuments_IndexIsEmpty()
    {
        _searchEngine.RemoveAll();

        var result = _searchEngine.Search(new AllFieldsSearchQuery()).Items.ToList();

        Assert.That(result, Has.Count.EqualTo(0));
    }

    [Test]
    public void AddRange_AddThreeNewDocuments_IndexHasSixDocuments()
    {
        var posts = new List<Post>
        {
            new()
            {
                Id = 1,
                Title = "Post about Search Engines",
                Content = "This is a test post"
            },
            new()
            {
                Id = 2,
                Title = "Just another post about Search Engines",
                Content = "Another test post"
            },
            new()
            {
                Id = 2,
                Title = "Search is cool with Apache Lucene.NET",
                Content = "<h1>Apache Lucene at the core!</h1>"
            }
        };

        _searchEngine.AddRange(posts);

        var result = _searchEngine.Search(new AllFieldsSearchQuery()).Items.ToList();

        Assert.That(result, Has.Count.EqualTo(6));
    }

    [Test]
    public void Search_FullTextSearchQueryTypoInSearchTerm_ReturnsOnePost()
    {
        var query = new FullTextSearchQuery { SearchTerm = "foy" };

        var result = _searchEngine.Search(query);

        Assert.That(result.Items.Count(), Is.EqualTo(1));
    }
}