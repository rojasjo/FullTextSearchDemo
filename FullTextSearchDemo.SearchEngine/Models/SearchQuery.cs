namespace FullTextSearchDemo.SearchEngine.Models;

public abstract class SearchQuery
{
    private const int DefaultPageSize = 10;
    private readonly int _pageSize = DefaultPageSize;
    private readonly int _pageNumber;
    
    /// <summary>
    /// The type of search query.
    /// </summary>
    public SearchType Type { get; init; }
    
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
}