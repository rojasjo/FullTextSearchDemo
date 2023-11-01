using FullTextSearchDemo.SearchEngine.Helpers;
using FullTextSearchDemo.SearchEngine.Models;
using Lucene.Net.Index;
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
        var query = ConstructQuery(searchQuery.SearchTerms, searchQuery.Type);
        return PerformSearch(query, searchQuery.PageNumber, searchQuery.PageSize);
    }

    public SearchResult<T> Search(AllFieldsSearchQuery searchQuery)
    {
        var instance = Activator.CreateInstance<T>();

        // Search all string properties for the search term
        var searchDictionary = typeof(T).GetProperties().Select(property => property.Name)
            .Select(fieldName => new { fieldName, type = instance.GetType().GetProperty(fieldName)?.PropertyType })
            .Where(p => p.fieldName != nameof(IDocument.UniqueKey))
            .Where(t => t.type != null)
            .Where(t => t.type == string.Empty.GetType())
            .Select(t => t.fieldName).ToDictionary(fieldName => fieldName, _ => searchQuery.SearchTerm);

        var query = ConstructQuery(searchDictionary, searchQuery.Type);

        return PerformSearch(query, searchQuery.PageNumber, searchQuery.PageSize);
    }

    private SearchResult<T> PerformSearch(Query query, int pageNumber, int pageSize)
    {
        var searchTopDocs = _searcher.Search(query, int.MaxValue);
        var documents = searchTopDocs.ScoreDocs;

        var result = new SearchResult<T>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = searchTopDocs.TotalHits
        };

        var start = pageNumber * pageSize;
        var end = Math.Min(start + pageSize, documents.Length);

        if (start > end)
        {
            result.Items = Enumerable.Empty<T>();
        }
        else
        {
            result.Items = documents[start..end].Select(hit => _searcher.Doc(hit.Doc))
                .Select(d => d.ConvertToObjectOfType<T>());
        }

        return result;
    }

    private static Query ConstructQuery(IDictionary<string, string?>? searchFiles, SearchType searchType)
    {
        if (searchFiles == null || searchFiles.Count == 0)
        {
            return new MatchAllDocsQuery();
        }

        if (searchFiles.All(p => string.IsNullOrWhiteSpace(p.Value)))
        {
            return new MatchAllDocsQuery();
        }

        var query = new BooleanQuery();
        var instance = Activator.CreateInstance<T>();
        foreach (var (fieldName, value) in searchFiles)
        {
            var type = instance.GetType().GetProperty(fieldName)?.PropertyType;

            if (type == null || type != typeof(string))
            {
                continue;
            }

            Query searchQuery = searchType switch
            {
                SearchType.ExactMatch => new TermQuery(new Term(fieldName, value)),
                SearchType.PrefixMatch => new PrefixQuery(new Term(fieldName, value)),
                SearchType.FuzzyMatch => new FuzzyQuery(new Term(fieldName, value)),
                _ => new TermQuery(new Term(fieldName, value))
            };

            query.Add(searchQuery, Occur.SHOULD);
        }

        return query;
    }
}