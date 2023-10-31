using System.Runtime.CompilerServices;
using FullTextSearchDemo.SearchEngine.Models;
using FullTextSearchDemo.SearchEngine.Services;

[assembly: InternalsVisibleTo("FullTextSearchDemo.SearchEngine.Tests")]

namespace FullTextSearchDemo.SearchEngine.Engine;

internal class SearchEngine<T> : ISearchEngine<T> where T : class
{
    private readonly IDocumentReader<T> _documentReader;
    private readonly IDocumentWriter<T> _documentWriter;
    
    public SearchEngine(IDocumentReader<T> documentReader, IDocumentWriter<T> documentWriter)
    {
        _documentReader = documentReader;
        _documentWriter = documentWriter;
    }

    public IEnumerable<T> Search(FieldSpecificSearchQuery searchQuery)
    {
        return _documentReader.Search(searchQuery);
    }

    public IEnumerable<T> Search(AllFieldsSearchQuery searchQuery)
    {
        return _documentReader.Search(searchQuery);
    }

    public void Add(T document)
    {
        _documentWriter.WriteDocument(document);
    }
}