using FullTextSearchDemo.SearchEngine.Models;
using Lucene.Net.Index;

namespace FullTextSearchDemo.SearchEngine.Services;

internal interface IDocumentWriter<in T> where T : IDocument
{
    void AddDocument(T generic);

    void UpdateDocument(T generic);

    void RemoveDocument(T generic);
    
    IndexWriter Writer { get; }
}