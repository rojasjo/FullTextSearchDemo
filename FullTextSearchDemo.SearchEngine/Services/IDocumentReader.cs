using FullTextSearchDemo.SearchEngine.Models;
using FullTextSearchDemo.SearchEngine.Queries;
using FullTextSearchDemo.SearchEngine.Results;

namespace FullTextSearchDemo.SearchEngine.Services;

internal interface IDocumentReader<T> where T : IDocument
{
    SearchResult<T> Search(FieldSpecificSearchQuery searchQuery);

    SearchResult<T> Search(AllFieldsSearchQuery searchQuery);

    SearchResult<T> Search(FullTextSearchQuery searchQuery);
}