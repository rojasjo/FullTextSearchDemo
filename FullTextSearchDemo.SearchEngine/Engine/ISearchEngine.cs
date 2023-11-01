using FullTextSearchDemo.SearchEngine.Models;

namespace FullTextSearchDemo.SearchEngine.Engine;

public interface ISearchEngine<T> where T : IDocument
{
    void Add(T document);

    void Update(T document);

    void Remove(T document);
    
    public SearchResult<T> Search(FieldSpecificSearchQuery searchQuery);
    
    public SearchResult<T> Search(AllFieldsSearchQuery searchQuery);
}