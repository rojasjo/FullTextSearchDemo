namespace FullTextSearchDemo.SearchEngine.Queries;

/// <summary>
/// Enable to configure the kind of search to be performed.
/// </summary>
public enum SearchType
{
    FuzzyMatch,
    ExactMatch,
    PrefixMatch
}