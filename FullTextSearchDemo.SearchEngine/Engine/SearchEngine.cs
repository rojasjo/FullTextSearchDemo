using System.Runtime.CompilerServices;
using FullTextSearchDemo.SearchEngine.Models;
using FullTextSearchDemo.SearchEngine.Services;

[assembly: InternalsVisibleTo("FullTextSearchDemo.SearchEngine.Tests")]

namespace FullTextSearchDemo.SearchEngine.Engine;

internal class SearchEngine<T> : ISearchEngine<T> where T : class
{
    private readonly IDocumentFactory<T> _documentFactory;

    public SearchEngine(IDocumentFactory<T> documentFactory)
    {
        _documentFactory = documentFactory;
    }

    public IEnumerable<T> Search(FieldSpecificSearchQuery searchQuery)
    {
        var documentReader = _documentFactory.CreateDocumentReader();
        return documentReader.Search<T>(searchQuery);
    }

    public IEnumerable<T> Search(AllFieldsSearchQuery searchQuery)
    {
        var documentReader = _documentFactory.CreateDocumentReader();
        return documentReader.Search<T>(searchQuery);
    }

    public void Add(T document)
    {
        var documentWriter = _documentFactory.CreateDocumentWriter();
        documentWriter.WriteDocument(document);
    }
}