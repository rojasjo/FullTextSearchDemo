using FullTextSearchDemo.SearchEngine.Models;

namespace FullTextSearchDemo.SearchEngine.Services;

internal interface IDocumentReader<T> where T : IDocument
{
    public SearchResult<T> Search(FieldSpecificSearchQuery searchQuery);

    public SearchResult<T> Search(AllFieldsSearchQuery searchQuery);
}