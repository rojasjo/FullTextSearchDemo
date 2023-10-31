using FullTextSearchDemo.SearchEngine.Configuration;
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
    private readonly string _index;

    public IndexWriter Writer { get; }

    public DocumentWriter(IIndexConfiguration<T> configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.IndexName))
        {
            throw new ArgumentException("Index name must be set before using DocumentWriter.");
        }

        _index = configuration.IndexName;
        
        // Open the index directory
        var indexPath = Path.Combine(Environment.CurrentDirectory, _index);
        LuceneDirectory indexDir = FSDirectory.Open(indexPath);

        // Create an analyzer to process the text
        const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;
        Analyzer standardAnalyzer = new StandardAnalyzer(luceneVersion);
        var indexConfig = new IndexWriterConfig(luceneVersion, standardAnalyzer)
        {
            OpenMode = OpenMode.CREATE_OR_APPEND
        };

        // Create the index writer with the above configuration
        Writer = new IndexWriter(indexDir, indexConfig);
    }
    
    public void WriteDocument(T generic)
    {
        var document = generic.ConvertToDocument();
        WriteDocument(document);
    }
    
    private void WriteDocument(Document document)
    {
        Writer.AddDocument(document);
        Writer.Commit();
    }
}