using FullTextSearchDemo.SearchEngine.Helpers;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace FullTextSearchDemo.SearchEngine.Services;

internal sealed class DocumentWriter<T> : IDocumentWriter<T> where T : class
{
    private static readonly Lazy<DocumentWriter<T>> DocumentWriterInstance = new(() => new DocumentWriter<T>());

    public static DocumentWriter<T> Instance => DocumentWriterInstance.Value;

    public static string? Index { get; set; }

    public IndexWriter Writer { get; }

    private DocumentWriter()
    {
        if (string.IsNullOrWhiteSpace(Index))
        {
            throw new ArgumentException("Index name must be set before using DocumentWriter.");
        }

        var indexPath = Path.Combine(Environment.CurrentDirectory, Index);

        LuceneDirectory indexDir = FSDirectory.Open(indexPath);

        const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;
        Analyzer standardAnalyzer = new StandardAnalyzer(luceneVersion);

        var indexConfig = new IndexWriterConfig(luceneVersion, standardAnalyzer)
        {
            OpenMode = OpenMode.CREATE_OR_APPEND
        };

        Writer = new IndexWriter(indexDir, indexConfig);
    }

    public void WriteDocument(Document document)
    {
        Writer.AddDocument(document);
        Writer.Commit();
    }

    public void WriteDocument(T generic)
    {
        var document = generic.ConvertToDocument();
        WriteDocument(document);
    }
}