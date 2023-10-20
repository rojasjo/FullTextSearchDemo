namespace FullTextSearchDemo.SearchEngine.Services;

internal interface IDocumentFactory<T> where T : class
{
    IDocumentReader CreateDocumentReader();

    IDocumentWriter<T> CreateDocumentWriter();
}