using FullTextSearchDemo.SearchEngine.Facets;
using FullTextSearchDemo.SearchEngine.Models;

namespace FullTextSearchDemo.SearchEngine.Results;

/// <summary>
/// Represents the result of a search query, including a list of items, page-related information,
/// and total item count.
/// </summary>
public class SearchResult<T> where T : IDocument
{
    /// <summary>
    /// The list of items found by the search query.
    /// </summary>
    public IEnumerable<T> Items { get; init; }
    
    /// <summary>
    /// The page number from which the items are retrieved.
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// The number of items contained on each page.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// The total count of items that match the search query.
    /// </summary>
    public int TotalItems { get; init; }

    /// <summary>
    /// The total number of pages based on the total item count and page size.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
    
    /// <summary>
    /// Facet results for the search query.
    /// </summary>
    public IEnumerable<FacetFilter> Facets { get; set; }
}