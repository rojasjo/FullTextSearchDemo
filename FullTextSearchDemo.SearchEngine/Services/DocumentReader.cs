using FullTextSearchDemo.SearchEngine.Configuration;
using FullTextSearchDemo.SearchEngine.Helpers;
using FullTextSearchDemo.SearchEngine.Models;
using Lucene.Net.Index;
using Lucene.Net.Search;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace FullTextSearchDemo.SearchEngine.Services;

internal sealed class DocumentReader : IDocumentReader
{
    private readonly IndexSearcher _searcher;

    public DocumentReader(IndexReader directoryReader)
    {
        _searcher = new IndexSearcher(directoryReader);
    }

    public IEnumerable<T> Search<T>(FieldSpecificSearchQuery searchQuery) where T : class
    {
        var query = ConstructQuery<T>(searchQuery.SearchTerms, searchQuery.Type);

        return PerformSearch<T>(query, searchQuery.PageNumber, searchQuery.PageSize);
    }

    public IEnumerable<T> Search<T>(AllFieldsSearchQuery searchQuery) where T : class
    {
        var instance = Activator.CreateInstance<T>();

        // Search all string properties for the search term
        var searchDictionary = typeof(T).GetProperties().Select(property => property.Name)
            .Select(fieldName => new { fieldName, type = instance.GetType().GetProperty(fieldName)?.PropertyType })
            .Where(t => t.type != null)
            .Where(t => t.type == string.Empty.GetType())
            .Select(t => t.fieldName).ToDictionary(fieldName => fieldName, fieldName => searchQuery.SearchTerm);

        var query = ConstructQuery<T>(searchDictionary, searchQuery.Type);

        return PerformSearch<T>(query, searchQuery.PageNumber, searchQuery.PageSize);
    }

    private IEnumerable<T> PerformSearch<T>(Query query, int pageNumber, int pageSize) where T : class
    {
        var scoredDocs = _searcher.Search(query, int.MaxValue).ScoreDocs;

        var start = pageNumber * pageSize;
        var end = Math.Min(start + pageSize, scoredDocs.Length);

        if (start > end)
        {
            return Enumerable.Empty<T>();
        }
        
        return scoredDocs[start..end].Select(hit => _searcher.Doc(hit.Doc)).Select(d => d.ConvertToObjectOfType<T>())
            .ToList();
    }

    private static Query ConstructQuery<T>(IDictionary<string, string>? searchFiles, SearchType searchType)
        where T : class
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

            Query? searchQuery = searchType switch
            {
                SearchType.ExactMatch => new TermQuery(new Term(fieldName, value)),
                SearchType.PrefixMatch => new PrefixQuery(new Term(fieldName, value)),
                SearchType.FuzzyMatch => new FuzzyQuery(new Term(fieldName, value)),
                _ => null
            };

            query.Add(searchQuery, Occur.SHOULD);
        }

        return query;
    }
}