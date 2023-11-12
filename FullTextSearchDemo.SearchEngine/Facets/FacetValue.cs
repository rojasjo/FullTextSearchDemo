namespace FullTextSearchDemo.SearchEngine.Facets;

public class FacetValue
{
    /// <summary>
    /// Value associated with the facet
    /// </summary>
    public string? Value { get; set; }
    
    /// <summary>
    /// Number of documents associated with the facet value
    /// </summary>
    public int Count { get; set; }
}