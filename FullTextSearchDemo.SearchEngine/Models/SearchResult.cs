namespace FullTextSearchDemo.SearchEngine.Models;

public class SearchResult<T>
{
    public IEnumerable<T> Items { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalItems { get; set; }

    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
}