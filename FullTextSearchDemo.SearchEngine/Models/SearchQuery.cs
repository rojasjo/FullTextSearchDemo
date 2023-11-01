namespace FullTextSearchDemo.SearchEngine.Models;

public abstract class SearchQuery
{
    private const int DefaultPageSize = 10;

    public SearchType Type { get; set; }

    private int _pageNumber;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 0 ? 0 : value;
    }

    private int _pageSize = DefaultPageSize;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value <= 0 ? DefaultPageSize : value;
    }
}