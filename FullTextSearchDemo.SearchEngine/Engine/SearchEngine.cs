using System.Runtime.CompilerServices;
using FullTextSearchDemo.SearchEngine.Models;
using FullTextSearchDemo.SearchEngine.Queries;
using FullTextSearchDemo.SearchEngine.Results;
using FullTextSearchDemo.SearchEngine.Services;
using Lucene.Net.Index;

[assembly: InternalsVisibleTo("FullTextSearchDemo.SearchEngine.Tests")]

namespace FullTextSearchDemo.SearchEngine.Engine;

internal class SearchEngine<T> : ISearchEngine<T> where T : IDocument
{
    private readonly IDocumentReader<T> _documentReader;
    private readonly IDocumentWriter<T> _documentWriter;

    public SearchEngine(IDocumentReader<T> documentReader, IDocumentWriter<T> documentWriter)
    {
        _documentReader = documentReader;
        _documentWriter = documentWriter;
        _documentWriter.Init();
    }

    public void Add(T document)
    {
        _documentWriter.AddDocument(document);
    }

    public void AddRange(IEnumerable<T> documents)
    {
        _documentWriter.AddDocuments(documents);
    }

    public void DisposeResources()
    {
        _documentWriter.Dispose();
    }
    
    public void Update(T document)
    {
        _documentWriter.UpdateDocument(document);
    }

    public void Remove(T document)
    {
        _documentWriter.RemoveDocument(document);
    }

    public SearchResult<T> Search(SearchQuery searchQuery)
    {
        try
        {
            return searchQuery switch
            {
                FieldSpecificSearchQuery fieldSpecificSearchQuery => Search(fieldSpecificSearchQuery),
                AllFieldsSearchQuery allFieldsSearchQuery => Search(allFieldsSearchQuery),
                FullTextSearchQuery fullTextSearchQuery => Search(fullTextSearchQuery),
                _ => throw new ArgumentException($"Invalid search query type: {searchQuery.GetType().Name}")
            };
        }
        catch (IndexNotFoundException)
        {
            return new SearchResult<T>
            {
                Items = Enumerable.Empty<T>(),
                PageNumber = searchQuery.PageNumber,
                PageSize = searchQuery.PageSize,
                TotalItems = 0
            };
        }
    }
    
    public void RemoveAll()
    {
        _documentWriter.Clear();
    }

    private SearchResult<T> Search(FieldSpecificSearchQuery searchQuery)
    {
        return _documentReader.Search(searchQuery);
    }

    private SearchResult<T> Search(AllFieldsSearchQuery searchQuery)
    {
        return _documentReader.Search(searchQuery);
    }

    private SearchResult<T> Search(FullTextSearchQuery searchQuery)
    {
        return _documentReader.Search(searchQuery);
    }
}