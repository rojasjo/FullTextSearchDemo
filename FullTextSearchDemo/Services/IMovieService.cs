using FullTextSearchDemo.Models;
using FullTextSearchDemo.Parameters;
using FullTextSearchDemo.SearchEngine.Results;

namespace FullTextSearchDemo.Services;

public interface IMovieService
{
    SearchResult<Movie> GetMovies(GetMoviesQuery query);
    
    SearchResult<Movie> SearchMovies(SearchMovieQuery query);
    
    SearchResult<Movie> FullTextSearchMovies(SearchMovieQuery query);
}