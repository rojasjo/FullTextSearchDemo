using FullTextSearchDemo.Models;
using FullTextSearchDemo.Parameters;
using FullTextSearchDemo.SearchEngine;
using FullTextSearchDemo.SearchEngine.Queries;
using Microsoft.AspNetCore.Mvc;

namespace FullTextSearchDemo.Controllers;

[ApiController]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
    private readonly ISearchEngine<Movie> _searchEngine;

    public MoviesController(ISearchEngine<Movie> searchEngine)
    {
        _searchEngine = searchEngine;
    }

    [HttpGet]
    public IActionResult GetMovies([FromQuery] GetMoviesQuery query)
    {
        try
        {
            var searchFields = new Dictionary<string, string?>();

            if (query.PrimaryTitle != null)
            {
                searchFields.Add(nameof(query.PrimaryTitle), query.PrimaryTitle);
            }

            if (query.OriginalTitle != null)
            {
                searchFields.Add(nameof(query.OriginalTitle), query.OriginalTitle);
            }

            if (query.TitleType != null)
            {
                searchFields.Add(nameof(query.TitleType), query.TitleType);
            }

            if (query.IsAdult != null)
            {
                searchFields.Add(nameof(query.IsAdult), query.IsAdult.ToString());
            }

            if (query.StartYear != null)
            {
                searchFields.Add(nameof(query.StartYear), query.StartYear.ToString());
            }

            if (query.EndYear != null)
            {
                searchFields.Add(nameof(query.EndYear), query.EndYear.ToString());
            }

            if (query.RuntimeMinutes != null)
            {
                searchFields.Add(nameof(query.RuntimeMinutes), query.RuntimeMinutes.ToString());
            }

            if (query.Genres != null)
            {
                searchFields.Add(nameof(query.Genres), string.Join(",", query.Genres));
            }

            var result = _searchEngine.Search(new FieldSpecificSearchQuery
            {
                SearchTerms = searchFields,
                Type = SearchType.PrefixMatch,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpGet("search")]
    public IActionResult SearchMovies([FromQuery] SearchMovieQuery query)
    {
        try
        {
            var result = _searchEngine.Search(new AllFieldsSearchQuery
            {
                SearchTerm = query.Term,
                Type = SearchType.PrefixMatch,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpGet("fulltextsearch")]
    public IActionResult FullTextSearchMovies([FromQuery] SearchMovieQuery query)
    {
        try
        {
            var result = _searchEngine.Search(new FullTextSearchQuery
            {
                SearchTerm = query.Term,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            });
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }
}