namespace FullTextSearchDemo.Parameters;

public class ProductsSearchQuery : QueryParameters
{
    public string? Search { get; set; }
    
    public string[]? Categories { get; set; }
    
    public bool? InSale { get; set; }
}