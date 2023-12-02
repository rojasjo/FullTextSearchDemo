using FullTextSearchDemo.SearchEngine.Models;
using Lucene.Net.Facet;

namespace FullTextSearchDemo.SearchEngine.Configuration;

public class FacetConfiguration<T> where T : IDocument
{
    public IEnumerable<string>? MultiValuedFields { set; get; }
    
    public required string IndexName { get; set; }

    internal FacetsConfig GetFacetConfig()
    {
        var facetsConfig = new FacetsConfig();
        
        if (MultiValuedFields == null)
        {
            return facetsConfig;
        }
        
        foreach (var field in MultiValuedFields)
        {
            facetsConfig.SetMultiValued(field, true);
        }
        
        return facetsConfig;
    }
}