using FullTextSearchDemo.Models;
using FullTextSearchDemo.Parameters;
using FullTextSearchDemo.SearchEngine.Models;
using FullTextSearchDemo.SearchEngine.Queries;
using FullTextSearchDemo.SearchEngine.Results;

namespace FullTextSearchDemo.Services;

public interface IProductService
{
    SearchResult<Product> GetProducts(GetProductsQuery query);

    SearchResult<Product> SearchProducts(ProductsSearchQuery query);
    
    void Add(Product product);

    void Update(int id, Product product);

    void Delete(int id);
    
    SearchResult<Product> FullSearchProducts(ProductsSearchQuery query);
}