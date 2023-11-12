using FullTextSearchDemo.SearchEngine.Facets;
using FullTextSearchDemo.SearchEngine.Models;

namespace FullTextSearchDemo.SearchEngine.Tests.TestModels;

public class Vehicle : IDocument
{
    public string UniqueKey => Id.ToString();
    
    public int Id { get; set; }
    
    [FacetProperty]
    public required string Name { get; set; }
    
    [FacetProperty]
    public required string Type { get; set; }
    
    [FacetProperty]
    public required string Brand { get; set; }
    
    [FacetProperty]
    public required string Model { get; set; }
    
    [FacetProperty]
    public int Year { get; set; }
    
}