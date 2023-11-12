namespace FullTextSearchDemo.Parameters;

public class MoviesQuery : QueryParameters
{
    public string[]? TitleTypeFacets { get; set; }
    
    public string[]? FacetGenreFacets { get; set; }
}