using FullTextSearchDemo.Models;
using FullTextSearchDemo.Parameters;
using FullTextSearchDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace FullTextSearchDemo.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public IActionResult GetProducts([FromQuery] GetProductsQuery query)
    {
        try
        {
            var result = _productService.GetProducts(query);
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
            var result = _productService.SearchProducts(query);
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
            _productService.Add(product);
            return Ok("Product added to the search index.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }
}