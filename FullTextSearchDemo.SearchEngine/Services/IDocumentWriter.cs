using System.Diagnostics.CodeAnalysis;
using FullTextSearchDemo.SearchEngine.Models;
using Lucene.Net.Facet;
using Lucene.Net.Index;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace FullTextSearchDemo.SearchEngine.Services;

internal interface IDocumentWriter<in T> where T : IDocument
{
    void AddDocument([NotNull] T generic);

    void UpdateDocument([NotNull] T generic);

    void RemoveDocument([NotNull] T generic);
    
    IndexWriter Writer { get; }
    
    FacetsConfig? FacetsConfig { get; }
    
    LuceneDirectory FacetIndexDirectory { get; }
    
    void Clear();

    void AddDocuments(IEnumerable<T> documents);
}