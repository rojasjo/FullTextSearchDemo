using FullTextSearchDemo.Parameters;
using FullTextSearchDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace FullTextSearchDemo.Controllers;

[ApiController]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpGet]
    public IActionResult GetMovies([FromQuery] GetMoviesQuery query)
    {
        try
        {
            var result = _movieService.GetMovies(query);
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
            var result = _movieService.SearchMovies(query);
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
            var result = _movieService.FullTextSearchMovies(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }
}