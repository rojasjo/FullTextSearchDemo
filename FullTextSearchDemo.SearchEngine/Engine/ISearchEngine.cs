using FullTextSearchDemo.SearchEngine.Models;

namespace FullTextSearchDemo.SearchEngine.Engine;

public interface ISearchEngine<T> where T : IDocument
{
    void Add(T document);

    void Update(T document);

    void Remove(T document);
    
    public IEnumerable<T> Search(FieldSpecificSearchQuery searchQuery);
    
    public IEnumerable<T> Search(AllFieldsSearchQuery searchQuery);
}