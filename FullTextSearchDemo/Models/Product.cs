using FullTextSearchDemo.SearchEngine.Facets;
using FullTextSearchDemo.SearchEngine.Models;

namespace FullTextSearchDemo.Models;

public class Product : IDocument
{
    public string UniqueKey => Id.ToString();
    
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public string? Description { get; set; }
    
    public float Price { get; set; }

    [FacetProperty]
    public string? Category { get; set; }
    
    [FacetProperty]
    public bool InSale { get; set; }
}