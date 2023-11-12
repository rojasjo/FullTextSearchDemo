using FullTextSearchDemo.SearchEngine.Configuration;

namespace FullTextSearchDemo.SearchEngine.Tests.TestModels;

public class PostTestConfiguration : IIndexConfiguration<Post>
{
    public string IndexName => "post-test-index";
    
    public FacetConfiguration<Post>? FacetConfiguration { get; }
}