using System.Runtime.CompilerServices;
using FullTextSearchDemo.SearchEngine.Models;
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

    public void Update(T document)
    {
        _documentWriter.UpdateDocument(document);
    }

    public void Remove(T document)
    {
        _documentWriter.RemoveDocument(document);
    }

    public IEnumerable<T> Search(FieldSpecificSearchQuery searchQuery)
    {
        return _documentReader.Search(searchQuery);
    }

    public IEnumerable<T> Search(AllFieldsSearchQuery searchQuery)
    {
        return _documentReader.Search(searchQuery);
    }
}