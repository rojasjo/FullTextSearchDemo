using FullTextSearchDemo.SearchEngine.Models;

namespace FullTextSearchDemo.SearchEngine.Services;

internal interface IDocumentReader
{
    public IEnumerable<T> Search<T>(FieldSpecificSearchQuery searchQuery)
        where T : class;
    
    public IEnumerable<T> Search<T>(AllFieldsSearchQuery searchQuery) where T : class;
}