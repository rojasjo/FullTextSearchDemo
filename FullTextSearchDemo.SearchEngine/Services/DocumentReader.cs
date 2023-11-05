using FullTextSearchDemo.SearchEngine.Helpers;
using FullTextSearchDemo.SearchEngine.Models;
using FullTextSearchDemo.SearchEngine.Queries;
using FullTextSearchDemo.SearchEngine.Results;
using Lucene.Net.Search;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace FullTextSearchDemo.SearchEngine.Services;

internal sealed class DocumentReader<T> : IDocumentReader<T> where T : IDocument
{
    private readonly IndexSearcher _searcher;

    public DocumentReader(IDocumentWriter<T> documentWriter)
    {
        var reader = documentWriter.Writer.GetReader(true);
        _searcher = new IndexSearcher(reader);
    }

    public SearchResult<T> Search(FieldSpecificSearchQuery searchQuery)
    {
        var query = LuceneQueryBuilder.ConstructQuery<T>(searchQuery.SearchTerms, searchQuery.Type);
        return PerformSearch(query, searchQuery.PageNumber, searchQuery.PageSize);
    }

    public SearchResult<T> Search(AllFieldsSearchQuery searchQuery)
    {
        var searchDictionary = DocumentFieldsHelper.GetStringField<T>()
            .ToDictionary(fieldName => fieldName, _ => searchQuery.SearchTerm);

        var query = LuceneQueryBuilder.ConstructQuery<T>(searchDictionary, searchQuery.Type);

        return PerformSearch(query, searchQuery.PageNumber, searchQuery.PageSize);
    }

    public SearchResult<T> Search(FullTextSearchQuery searchQuery)
    {
        Query query = new MatchAllDocsQuery();

        if (!string.IsNullOrWhiteSpace(searchQuery.SearchTerm))
        {
            query = LuceneQueryBuilder.ConstructFulltextSearchQuery<T>(searchQuery);
        }

        return PerformSearch(query, searchQuery.PageNumber, searchQuery.PageSize);
    }

    private SearchResult<T> PerformSearch(Query query, int pageNumber, int pageSize)
    {
        var searchTopDocs = _searcher.Search(query, int.MaxValue);
        var documents = searchTopDocs.ScoreDocs;

        var start = pageNumber * pageSize;
        var end = Math.Min(start + pageSize, documents.Length);

        IEnumerable<T> items;
        if (start > end)
        {
            items = Enumerable.Empty<T>();
        }
        else
        {
            items = documents[start..end].Select(hit => _searcher.Doc(hit.Doc))
                .Select(d => d.ConvertToObjectOfType<T>());
        }

        return new SearchResult<T>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = searchTopDocs.TotalHits
        };
    }
}