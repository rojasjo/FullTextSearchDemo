using System.Diagnostics.CodeAnalysis;
using FullTextSearchDemo.SearchEngine.Models;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace FullTextSearchDemo.SearchEngine.Services;

internal interface IDocumentWriter<in T> where T : IDocument
{
    void AddDocument([NotNull] T generic);

    void AddDocuments(IEnumerable<T> documents);
    
    void Clear();
    
    void Dispose();
    
    void Init();
    
    void UpdateDocument([NotNull] T generic);

    void RemoveDocument([NotNull] T generic);
}