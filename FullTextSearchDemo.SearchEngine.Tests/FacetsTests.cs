using FullTextSearchDemo.SearchEngine.Services;
using FullTextSearchDemo.SearchEngine.Tests.TestModels;
using FullTextSearchDemo.SearchEngine.Engine;
using FullTextSearchDemo.SearchEngine.Queries;

namespace FullTextSearchDemo.SearchEngine.Tests;

[TestFixture]
public class FacetsTests
{
    private SearchEngine<Vehicle> _searchEngine = null!;

    [SetUp]
    public void Setup()
    {
        var configuration = new VehicleConfiguration();
        _searchEngine = new SearchEngine<Vehicle>(new DocumentReader<Vehicle>(configuration),
            new DocumentWriter<Vehicle>(configuration));
        _searchEngine.AddRange(VehicleHelper.GenerateFixedVehicles());
    }

    [TearDown]
    public void TearDown()
    {
        _searchEngine.RemoveAll();
        _searchEngine.DisposeResources();
    }

    [Test]
    public void Search_WithBrandFilter_ReturnsOnlyExpectedVehicles()
    {
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
    
    [Test]
    public void Search_WithNullFilter_ReturnsAllVehicles()
    {
        var searchResult = _searchEngine.Search(new AllFieldsSearchQuery()
        {
            Facets = null,
            PageSize = 25
        });

        Assert.Multiple(() =>
        {
            Assert.That(searchResult.Items.Count(), Is.EqualTo(25));
            Assert.That(searchResult.TotalItems, Is.EqualTo(25));
        });
    }
    
    [Test]
    public void Search_WithFilterWithoutValue_ReturnsAllVehicles()
    {
        var searchResult = _searchEngine.Search(new AllFieldsSearchQuery()
        {
            Facets = new Dictionary<string, IEnumerable<string?>?>()
            {
                {"Brand", null}
            },
            PageSize = 25
        });

        Assert.Multiple(() =>
        {
            Assert.That(searchResult.Items.Count(), Is.EqualTo(25));
            Assert.That(searchResult.TotalItems, Is.EqualTo(25));
        });
    }
}