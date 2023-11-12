using FullTextSearchDemo.Models;
using FullTextSearchDemo.Parameters;
using FullTextSearchDemo.SearchEngine;
using FullTextSearchDemo.SearchEngine.Queries;
using FullTextSearchDemo.SearchEngine.Results;

namespace FullTextSearchDemo.Services;

public class MovieService : IMovieService
{
    private readonly ISearchEngine<Movie> _searchEngine;

    public MovieService(ISearchEngine<Movie> searchEngine)
    {
        _searchEngine = searchEngine;
    }

    public SearchResult<Movie> GetMovies(GetMoviesQuery query)
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

        var facets = GetFacets(query);

        return _searchEngine.Search(new FieldSpecificSearchQuery
        {
            SearchTerms = searchFields,
            Type = SearchType.PrefixMatch,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            Facets = facets
        });
    }

    public SearchResult<Movie> SearchMovies(SearchMovieQuery query)
    {
        var facets = GetFacets(query);

        return _searchEngine.Search(new AllFieldsSearchQuery
        {
            SearchTerm = query.Term,
            Type = SearchType.PrefixMatch,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            Facets = facets
        });
    }

    public SearchResult<Movie> FullTextSearchMovies(SearchMovieQuery query)
    {
        var facets = GetFacets(query);
        return _searchEngine.Search(new FullTextSearchQuery
        {
            SearchTerm = query.Term,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            Facets = facets
        });
    }

    private static IDictionary<string, IEnumerable<string?>?> GetFacets(MoviesQuery query)
    {
        var facets = new Dictionary<string, IEnumerable<string?>?>();
        if (query.FacetGenreFacets != null)
        {
            facets.Add(nameof(Movie.Genres), query.FacetGenreFacets);
        }

        if (query.TitleTypeFacets != null)
        {
            facets.Add(nameof(Movie.TitleType), query.TitleTypeFacets);
        }

        return facets;
    }
}