namespace FullTextSearchDemo.SearchEngine.Models;

/// <summary>
/// Represents a search query for searching in all string fields
/// </summary>
public class AllFieldsSearchQuery : SearchQuery
{
    /// <summary>
    /// Gets or sets the search term to be used for searching in all string fields.
    /// </summary>
    public string? SearchTerm { get; set; }
}