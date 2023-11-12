namespace FullTextSearchDemo.Parameters;

public class GetProductsQuery : QueryParameters
{
    public string? Name { get; set; }

    public string? Description { get; set; }
    
    public string[]? Categories { get; set; }
    
    public bool? InSale { get; set; }
}