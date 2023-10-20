using FullTextSearchDemo.Models;
using FullTextSearchDemo.Parameters;
using FullTextSearchDemo.SearchEngine.Configuration;
using FullTextSearchDemo.SearchEngine.Engine;
using FullTextSearchDemo.SearchEngine.Models;
using Microsoft.AspNetCore.Mvc;

namespace FullTextSearchDemo.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly ISearchEngine<Product> _searchEngine;

    public ProductController(ISearchEngine<Product> searchEngine)
    {
        _searchEngine = searchEngine;
    }

    [HttpGet]
    public IActionResult GetProducts([FromQuery] GetProductsQuery query)
    {
        try
        {
            var searchTerm = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                searchTerm.Add(nameof(Product.Name), query.Name.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(query.Description))
            {
                searchTerm.Add(nameof(Product.Description), query.Description.ToLower());
            }

            var searchQuery = new FieldSpecificSearchQuery()
            {
                SearchTerms = searchTerm,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                Type = SearchType.ExactMatch
            };
            
            var result = _searchEngine.Search(searchQuery);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpGet("search")]
    public IActionResult SearchProducts([FromQuery] ProductsSearchQuery query)
    {
        try
        {
            var searchQuery = new AllFieldsSearchQuery()
            {
                SearchTerm = query.Search,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                Type = SearchType.FuzzyMatch
            };
            
            var result = _searchEngine.Search(searchQuery);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpPost]
    public IActionResult PostProduct([FromBody] Product product)
    {
        try
        {
            // Add the product to the search engine index
            _searchEngine.Add(product);
            return Ok("Product added to the search index.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }
}