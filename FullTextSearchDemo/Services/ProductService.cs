using FullTextSearchDemo.Models;
using FullTextSearchDemo.Parameters;
using FullTextSearchDemo.SearchEngine;
using FullTextSearchDemo.SearchEngine.Queries;
using FullTextSearchDemo.SearchEngine.Results;

namespace FullTextSearchDemo.Services;

public class ProductService : IProductService
{
    private readonly ISearchEngine<Product> _searchEngine;

    public ProductService(ISearchEngine<Product> searchEngine)
    {
        _searchEngine = searchEngine;
    }

    public SearchResult<Product> GetProducts(GetProductsQuery query)
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

        var facets = AdjustFacets(query.Categories, query.InSale);

        var searchQuery = new FieldSpecificSearchQuery
        {
            SearchTerms = searchTerm,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            Type = SearchType.ExactMatch,
            Facets = facets
        };

        return _searchEngine.Search(searchQuery);
    }

    public SearchResult<Product> SearchProducts(ProductsSearchQuery query)
    {
        var facets = AdjustFacets(query.Categories, query.InSale);
        
        var searchQuery = new AllFieldsSearchQuery
        {
            SearchTerm = query.Search,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            Type = SearchType.FuzzyMatch,
            Facets = facets
        };

        return _searchEngine.Search(searchQuery);
    }

    public void Add(Product product)
    {
        _searchEngine.Add(product);
        _searchEngine.DisposeResources();
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

    public SearchResult<Product> FullSearchProducts(ProductsSearchQuery query)
    {
        var facets = AdjustFacets(query.Categories, query.InSale);
        
        return _searchEngine.Search(new FullTextSearchQuery
        {
            SearchTerm = query.Search,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            Facets = facets
        });
    }
    
    private static Dictionary<string, IEnumerable<string?>?> AdjustFacets(IList<string>? categories, bool? inSale)
    {
        var facets = new Dictionary<string, IEnumerable<string?>?>();
        if (categories != null && categories.Any())
        {
            facets.Add("Category", categories);
        }

        if (inSale.HasValue)
        {
            facets.Add("InSale", new[] { inSale.Value.ToString() });
        }

        return facets;
    }
}