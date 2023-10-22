using FullTextSearchDemo.Models;
using FullTextSearchDemo.Parameters;

namespace FullTextSearchDemo.Services;

public interface IProductService
{
    IEnumerable<Product> GetProducts(GetProductsQuery query);
    
    IEnumerable<Product> SearchProducts(ProductsSearchQuery query);
    
    void Add(Product product);
}