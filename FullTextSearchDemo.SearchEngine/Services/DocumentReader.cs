using FullTextSearchDemo.SearchEngine.Facets;
using FullTextSearchDemo.SearchEngine.Helpers;
using FullTextSearchDemo.SearchEngine.Models;
using FullTextSearchDemo.SearchEngine.Queries;
using FullTextSearchDemo.SearchEngine.Results;
using Lucene.Net.Facet;
using Lucene.Net.Facet.Taxonomy;
using Lucene.Net.Facet.Taxonomy.Directory;
using Lucene.Net.Search;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace FullTextSearchDemo.SearchEngine.Services;

internal sealed class DocumentReader<T> : IDocumentReader<T> where T : IDocument
{
    private readonly IndexSearcher _searcher;
    private readonly IDocumentWriter<T> _documentWriter;

    public DocumentReader(IDocumentWriter<T> documentWriter)
    {
        _documentWriter = documentWriter;
        var reader = _documentWriter.Writer.GetReader(true);
        _searcher = new IndexSearcher(reader);
    }

    public SearchResult<T> Search(FieldSpecificSearchQuery searchQuery)
    {
        var query = LuceneQueryBuilder.ConstructQuery<T>(searchQuery.SearchTerms, searchQuery.Type);

        query = AddFacetsQueries(searchQuery.Facets, query);

        return PerformSearch(query, searchQuery.PageNumber, searchQuery.PageSize);
    }

    public SearchResult<T> Search(AllFieldsSearchQuery searchQuery)
    {
        var searchDictionary = DocumentFieldsHelper.GetStringField<T>()
            .ToDictionary(fieldName => fieldName, _ => searchQuery.SearchTerm);

        var query = LuceneQueryBuilder.ConstructQuery<T>(searchDictionary, searchQuery.Type);
        query = AddFacetsQueries(searchQuery.Facets, query);
        
        return PerformSearch(query, searchQuery.PageNumber, searchQuery.PageSize);
    }

    public SearchResult<T> Search(FullTextSearchQuery searchQuery)
    {
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

    private Query AddFacetsQueries(IDictionary<string, IEnumerable<string?>?>? facets, Query query)
    {
        if (_documentWriter.FacetsConfig == null)
        {
            return query;
        }

        if (facets == null)
        {
            return query;
        }
        
        var drillDownQuery = new DrillDownQuery(_documentWriter.FacetsConfig, query);
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

    private void SetFacetResults(Query query, SearchResult<T> result)
    {
        if (_documentWriter.FacetsConfig == null)
        {
            return;
        }

        var facetsCollector = new FacetsCollector();
        FacetsCollector.Search(_searcher, query, 100, facetsCollector);

        var directoryTaxonomyReader = new DirectoryTaxonomyReader(_documentWriter.FacetIndexDirectory);
        var facets =
            new FastTaxonomyFacetCounts(directoryTaxonomyReader, _documentWriter.FacetsConfig, facetsCollector);

        var facetResults = facets.GetAllDims(100).Select(facet => new FacetFilter
        {
            Name = facet.Dim,
            Values = facet.LabelValues.Select(p => new FacetValue { Value = p.Label, Count = (int)p.Value, })
        });

        result.Facets = facetResults;
    }
}