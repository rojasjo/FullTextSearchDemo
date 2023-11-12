using FullTextSearchDemo.Models;
using FullTextSearchDemo.SearchEngine.Configuration;

namespace FullTextSearchDemo.Search;

public class MoviesConfiguration : IIndexConfiguration<Movie>
{
    public string IndexName => "movies-index";

    public FacetConfiguration<Movie>? FacetConfiguration => new()
    {
        MultiValuedFields = new[] { nameof(Movie.Genres) }
    };
}