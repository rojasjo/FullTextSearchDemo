using Lucene.Net.Index;

namespace FullTextSearchDemo.SearchEngine.Services;

internal interface IDocumentWriter<in T> where T : class
{
    void WriteDocument(T generic);
    
    IndexWriter Writer { get; }
}