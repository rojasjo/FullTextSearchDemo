using FullTextSearchDemo.SearchEngine.Configuration;

namespace FullTextSearchDemo.SearchEngine.Services;

internal class DocumentFactory<T> : IDocumentFactory<T> where T : class
{
    public DocumentFactory(IIndexConfiguration<T> configuration)
    {
        DocumentWriter<T>.Index = configuration.IndexName;
    }

    public IDocumentReader CreateDocumentReader()
    {
        return new DocumentReader(DocumentWriter<T>.Instance.Writer.GetReader(true));
    }

    public IDocumentWriter<T> CreateDocumentWriter()
    {
        return DocumentWriter<T>.Instance;
    }
}