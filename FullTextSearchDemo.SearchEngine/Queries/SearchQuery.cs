namespace FullTextSearchDemo.SearchEngine.Queries;

public abstract class SearchQuery
{
    private const int DefaultPageSize = 10;
    private readonly int _pageSize = DefaultPageSize;
    private readonly int _pageNumber;
    
    /// <summary>
    /// The page number for the search query.
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber;
        init => _pageNumber = value < 0 ? 0 : value;
    }
    
    /// <summary>
    /// The page size for the search query.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value <= 0 ? DefaultPageSize : value;
    }
    
    /// <summary>
    /// Facets to be used for the search query.
    /// Example: "Facets": { "Category": [ "Books", "Movies" ], "Rating": [], "Author": ["", "Jimmy"] }
    /// Finds books and movies without ratings and from authors with the name Jimmy or no name.
    /// </summary>
    public IDictionary<string, IEnumerable<string?>?>? Facets { get; set; }
}