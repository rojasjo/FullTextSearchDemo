namespace FullTextSearchDemo.SearchEngine.Queries;

/// <summary>
/// Represents a search query for searching specific fields.
/// </summary>
public class FieldSpecificSearchQuery : ConfigurableQuery
{
    /// <summary>
    /// Gets or sets a dictionary of field-specific search terms where the keys represent the fields to search in,
    /// and the values represent the search terms.
    /// </summary>
    public IDictionary<string, string?>? SearchTerms { get; set; } = new Dictionary<string, string?>();
}