using FullTextSearchDemo.SearchEngine.Models;

namespace FullTextSearchDemo.SearchEngine.Engine;

public interface ISearchEngine<T> where T : class
{
    void Add(T document);

    public IEnumerable<T> Search(FieldSpecificSearchQuery searchQuery);
    
    public IEnumerable<T> Search(AllFieldsSearchQuery searchQuery);
}