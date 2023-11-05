namespace FullTextSearchDemo.SearchEngine.Queries;

/// <summary>
/// Performs a preconfigured search in all string fields.
/// The configuration uses a combination of the WildcardSearch and the FuzzySearch.
/// </summary>
public class FullTextSearchQuery : SearchQuery
{
    /// <summary>
    /// Gets or sets the search term to be used for searching in all string fields.
    /// </summary>
    public string? SearchTerm { get; set; }
}