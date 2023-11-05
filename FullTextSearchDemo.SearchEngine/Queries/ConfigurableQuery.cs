namespace FullTextSearchDemo.SearchEngine.Queries;

public abstract class ConfigurableQuery : SearchQuery
{
    /// <summary>
    /// The type of search query.
    /// </summary>
    public SearchType Type { get; init; }
}