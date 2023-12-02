using FullTextSearchDemo.SearchEngine.Configuration;
using FullTextSearchDemo.SearchEngine.Facets;
using FullTextSearchDemo.SearchEngine.Helpers;
using FullTextSearchDemo.SearchEngine.Models;
using FullTextSearchDemo.SearchEngine.Queries;
using FullTextSearchDemo.SearchEngine.Results;
using Lucene.Net.Facet;
using Lucene.Net.Facet.Taxonomy;
using Lucene.Net.Facet.Taxonomy.Directory;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace FullTextSearchDemo.SearchEngine.Services;

internal sealed class DocumentReader<T> : IDocumentReader<T> where T : IDocument
{
    private readonly IIndexConfiguration<T> _configuration;
    private DirectoryReader? _indexDirectoryReader;
    private IndexSearcher? _searcher;

    public DocumentReader(IIndexConfiguration<T> configuration)
    {
        _configuration = configuration;
    }

    public SearchResult<T> Search(FieldSpecificSearchQuery searchQuery)
    {
        Init();

        var query = LuceneQueryBuilder.ConstructQuery<T>(searchQuery.SearchTerms, searchQuery.Type);

        query = AddFacetsQueries(searchQuery.Facets, query);

        return PerformSearch(query, searchQuery.PageNumber, searchQuery.PageSize);
    }

    public SearchResult<T> Search(AllFieldsSearchQuery searchQuery)
    {
        Init();

        var searchDictionary = DocumentFieldsHelper.GetStringField<T>()
            .ToDictionary(fieldName => fieldName, _ => searchQuery.SearchTerm);

        var query = LuceneQueryBuilder.ConstructQuery<T>(searchDictionary, searchQuery.Type);
        query = AddFacetsQueries(searchQuery.Facets, query);

        return PerformSearch(query, searchQuery.PageNumber, searchQuery.PageSize);
    }


    public SearchResult<T> Search(FullTextSearchQuery searchQuery)
    {
        Init();

        Query query = new MatchAllDocsQuery();

        if (!string.IsNullOrWhiteSpace(searchQuery.SearchTerm))
        {
            query = LuceneQueryBuilder.ConstructFulltextSearchQuery<T>(searchQuery);
        }

        query = AddFacetsQueries(searchQuery.Facets, query);

        return PerformSearch(query, searchQuery.PageNumber, searchQuery.PageSize);
    }

    private SearchResult<T> PerformSearch(Query query, int pageNumber, int pageSize)
    {
        var searchTopDocs = _searcher!.Search(query, int.MaxValue);

        var items = GetItemsPaginated(pageNumber, pageSize, searchTopDocs);

        var result = new SearchResult<T>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = searchTopDocs.TotalHits
        };

        SetFacetResults(query, result);

        return result;
    }

    private IEnumerable<T> GetItemsPaginated(int pageNumber, int pageSize, TopDocs topDocs)
    {
        var documents = topDocs.ScoreDocs;
        var start = pageNumber * pageSize;
        var end = Math.Min(start + pageSize, documents.Length);

        IEnumerable<T> items;

        if (start > end)
        {
            items = Enumerable.Empty<T>();
        }
        else
        {
            items = documents[start..end].Select(hit => _searcher!.Doc(hit.Doc))
                .Select(d => d.ConvertToObjectOfType<T>());
        }

        return items;
    }
    
    private Query AddFacetsQueries(IDictionary<string, IEnumerable<string?>?>? facets, Query query)
    {
        if (_configuration.FacetConfiguration?.GetFacetConfig() == null)
        {
            return query;
        }

        if (facets == null)
        {
            return query;
        }

        var drillDownQuery = new DrillDownQuery(_configuration.FacetConfiguration.GetFacetConfig(), query);
        foreach (var facet in facets)
        {
            if (facet.Value == null || !facet.Value.Any())
            {
                continue;
            }

            foreach (var value in facet.Value.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray())
            {
                drillDownQuery.Add(facet.Key, value);
            }
        }

        return drillDownQuery;
    }

    private void Init()
    {
        var indexPath = Path.Combine(Environment.CurrentDirectory, _configuration.IndexName);
        _indexDirectoryReader = DirectoryReader.Open(FSDirectory.Open(indexPath));

        _searcher = new IndexSearcher(_indexDirectoryReader);
    }
    
    private void SetFacetResults(Query query, SearchResult<T> result)
    {
        if (_configuration.FacetConfiguration?.GetFacetConfig() == null)
        {
            return;
        }

        var facetsCollector = new FacetsCollector();
        FacetsCollector.Search(_searcher, query, 100, facetsCollector);
        using var facetsDirectory = FSDirectory.Open(_configuration.FacetConfiguration.IndexName);

        var directoryTaxonomyReader = new DirectoryTaxonomyReader(facetsDirectory);
        var facetConfig = _configuration.FacetConfiguration.GetFacetConfig();

        var facets = new FastTaxonomyFacetCounts(directoryTaxonomyReader, facetConfig,
            facetsCollector);

        var facetResults = facets.GetAllDims(100).Select(facet => new FacetFilter
        {
            Name = facet.Dim,
            Values = facet.LabelValues.Select(p => new FacetValue { Value = p.Label, Count = (int)p.Value, })
        });

        result.Facets = facetResults;
    }
}