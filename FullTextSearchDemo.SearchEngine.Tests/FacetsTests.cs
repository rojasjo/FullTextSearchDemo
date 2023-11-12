using FullTextSearchDemo.SearchEngine.Services;
using FullTextSearchDemo.SearchEngine.Tests.TestModels;
using FullTextSearchDemo.SearchEngine.Engine;
using FullTextSearchDemo.SearchEngine.Queries;

namespace FullTextSearchDemo.SearchEngine.Tests;

[TestFixture]
public class FacetsTests
{
    private readonly DocumentWriter<Vehicle> _documentWriter = new(new VehicleConfiguration());
    
    private SearchEngine<Vehicle> _searchEngine = null!;

    [SetUp]
    public void Setup()
    {
        _documentWriter.AddDocuments(VehicleHelper.GenerateFixedVehicles());
        
        _searchEngine = new SearchEngine<Vehicle>(new DocumentReader<Vehicle>(_documentWriter), _documentWriter);

    }
    
    [TearDown]
    public void TearDown()
    {
        _documentWriter.Clear();
    }
    
    [Test]
    public void Facet_FilterByBrand_ReturnsOnlyExpectedVehicles()
    {
        _searchEngine = new SearchEngine<Vehicle>(new DocumentReader<Vehicle>(_documentWriter), _documentWriter);

        var facets = new Dictionary<string, IEnumerable<string?>?>();
        facets.Add("Brand", new List<string?> { "Ford" });
        
        var searchResult = _searchEngine.Search(new AllFieldsSearchQuery()
        {
            Facets = facets
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(searchResult.Facets.ElementAt(0).Name, Is.EqualTo("Brand"));
            Assert.That(searchResult.Facets.ElementAt(0).Values!.Count(), Is.EqualTo(1));
            Assert.That(searchResult.Facets.ElementAt(0).Values!.ElementAt(0).Value, Is.EqualTo("Ford"));
            Assert.That(searchResult.Facets.ElementAt(0).Values!.ElementAt(0).Count, Is.EqualTo(3));
            Assert.That(searchResult.Items.Count(), Is.EqualTo(3));
            Assert.That(searchResult.Items.All(p => p.Brand == "Ford"), Is.EqualTo(true));
            Assert.That(searchResult.TotalItems, Is.EqualTo(3));
        });
    }
}