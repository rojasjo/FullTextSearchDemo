namespace FullTextSearchDemo.SearchEngine.Services;

internal interface IDocumentWriter<T> where T : class
{
    void WriteDocument(T generic);
}