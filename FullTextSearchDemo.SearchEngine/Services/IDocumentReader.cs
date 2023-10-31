using FullTextSearchDemo.SearchEngine.Models;

namespace FullTextSearchDemo.SearchEngine.Services;

internal interface IDocumentReader<out T> where T : IDocument
{
    public IEnumerable<T> Search(FieldSpecificSearchQuery searchQuery);

    public IEnumerable<T> Search(AllFieldsSearchQuery searchQuery);
}