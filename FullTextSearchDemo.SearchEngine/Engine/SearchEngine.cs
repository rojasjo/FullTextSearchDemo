using System.Runtime.CompilerServices;
using FullTextSearchDemo.SearchEngine.Models;
using FullTextSearchDemo.SearchEngine.Queries;
using FullTextSearchDemo.SearchEngine.Results;
using FullTextSearchDemo.SearchEngine.Services;

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
    }

    public void Add(T document)
    {
        _documentWriter.AddDocument(document);
    }

    public void AddRange(IEnumerable<T> documents)
    {
        _documentWriter.AddDocuments(documents);
    }

    public void Update(T document)
    {
        _documentWriter.UpdateDocument(document);
    }

    public void Remove(T document)
    {
        _documentWriter.RemoveDocument(document);
    }

    public SearchResult<T> Search(FieldSpecificSearchQuery searchQuery)
    {
        return _documentReader.Search(searchQuery);
    }

    public SearchResult<T> Search(AllFieldsSearchQuery searchQuery)
    {
        return _documentReader.Search(searchQuery);
    }

    public SearchResult<T> Search(FullTextSearchQuery searchQuery)
    {
        return _documentReader.Search(searchQuery);
    }

    public void Clear()
    {
        _documentWriter.Clear();
    }
}