namespace FullTextSearchDemo.SearchEngine.Models;

public class FieldSpecificSearchQuery : SearchQuery
{
    public IDictionary<string, string>? SearchTerms { get; set; } = new Dictionary<string, string>();
}