using FullTextSearchDemo.Models;
using FullTextSearchDemo.Parameters;
using FullTextSearchDemo.SearchEngine.Engine;
using FullTextSearchDemo.SearchEngine.Models;

namespace FullTextSearchDemo.Services;

public class ProductService : IProductService
{
    private readonly ISearchEngine<Product> _searchEngine;

    public ProductService(ISearchEngine<Product> searchEngine)
    {
        _searchEngine = searchEngine;
    }

    public IEnumerable<Product> GetProducts(GetProductsQuery query)
    {
        var searchTerm = new Dictionary<string, string?>();

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            searchTerm.Add(nameof(Product.Name), query.Name.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(query.Description))
        {
            searchTerm.Add(nameof(Product.Description), query.Description.ToLower());
        }

        var searchQuery = new FieldSpecificSearchQuery
        {
            SearchTerms = searchTerm,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            Type = SearchType.ExactMatch
        };

        return _searchEngine.Search(searchQuery);
    }

    public IEnumerable<Product> SearchProducts(ProductsSearchQuery query)
    {
        var searchQuery = new AllFieldsSearchQuery
        {
            SearchTerm = query.Search,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            Type = SearchType.FuzzyMatch
        };

        return _searchEngine.Search(searchQuery);
    }

    public void Add(Product product)
    {
        _searchEngine.Add(product);
    }

    public void Update(int id, Product product)
    {
        _searchEngine.Update(product);
    }

    public void Delete(int id)
    {
        //retrieve your item from the data store
        var product = new Product
        {
            Id = id,
            Name = "",
        };

        _searchEngine.Remove(product);
    }
}