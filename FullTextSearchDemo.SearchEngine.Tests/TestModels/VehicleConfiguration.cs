using FullTextSearchDemo.SearchEngine.Configuration;

namespace FullTextSearchDemo.SearchEngine.Tests.TestModels;

public class VehicleConfiguration : IIndexConfiguration<Vehicle>
{
    public string IndexName => "vehicle-test-index";

    public FacetConfiguration<Vehicle>? FacetConfiguration => new()
    {
        IndexName = "vehicle-test-index-facets"
    };
}