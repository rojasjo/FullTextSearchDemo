namespace FullTextSearchDemo.SearchEngine.Models;

public abstract class SearchQuery
{
    public SearchType Type { get; set; }

    public int PageNumber { get; set; } = 0;
    
    public int PageSize { get; set; } = 10;
}