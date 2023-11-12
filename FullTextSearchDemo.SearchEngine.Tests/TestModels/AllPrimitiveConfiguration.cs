using FullTextSearchDemo.SearchEngine.Configuration;

namespace FullTextSearchDemo.SearchEngine.Tests.TestModels;

public class AllPrimitiveConfiguration : IIndexConfiguration<Element>
{
    public string IndexName => "all-primitive-test-index";
    
    public FacetConfiguration<Element>? FacetConfiguration { get; }
}