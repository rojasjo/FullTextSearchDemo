using System.Diagnostics.CodeAnalysis;
using FullTextSearchDemo.SearchEngine.Configuration;
using FullTextSearchDemo.SearchEngine.Helpers;
using FullTextSearchDemo.SearchEngine.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Facet;
using Lucene.Net.Facet.Taxonomy.Directory;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace FullTextSearchDemo.SearchEngine.Services;

internal sealed class DocumentWriter<T> : IDisposable, IDocumentWriter<T> where T : IDocument
{
    private readonly FacetsConfig? _facetsConfig;
    private readonly string _indexName;
    private readonly string? _facetIndexName;
    
    private LuceneDirectory? _facetIndexDirectory;
    private bool _initialized;
    private DirectoryTaxonomyWriter? _taxonomyWriter;
    private IndexWriter? _writer;

    public DocumentWriter(IIndexConfiguration<T> configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.IndexName))
        {
            throw new ArgumentException("Index name must be set before using DocumentWriter.");
        }

        _indexName = configuration.IndexName;
        _facetIndexName = configuration.FacetConfiguration?.IndexName;
        _facetsConfig = configuration.FacetConfiguration?.GetFacetConfig();
    }

    public void Init()
    {
        if (_initialized)
        {
            return;
        }

        // Open the index directories
        var indexPath = Path.Combine(Environment.CurrentDirectory, _indexName);
        var indexDirectory = FSDirectory.Open(indexPath);

        // Create an analyzer to process the text
        const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;
        Analyzer standardAnalyzer = new StandardAnalyzer(luceneVersion);
        var indexConfig = new IndexWriterConfig(luceneVersion, standardAnalyzer)
        {
            OpenMode = OpenMode.CREATE_OR_APPEND,
        };

        // Create the index writer with the above configuration
        _writer = new IndexWriter(indexDirectory, indexConfig);

        if (_facetsConfig == null || string.IsNullOrWhiteSpace(_facetIndexName))
        {
            return;
        }

        _facetIndexDirectory = FSDirectory.Open(_facetIndexName);
        _taxonomyWriter = new DirectoryTaxonomyWriter(_facetIndexDirectory);

        _initialized = true;
    }

    public void AddDocument([NotNull] T generic)
    {
        var document = generic.ConvertToDocument();
        _writer?.AddDocument(GetDocument(document));

        Commit();
    }

    public void Clear()
    {
        _writer?.DeleteAll();

        Commit();
    }

    public void AddDocuments(IEnumerable<T> documents)
    {
        foreach (var generic in documents)
        {
           
            _writer?.AddDocument(GetDocument(generic));
        }

        Commit();
    }

    public void UpdateDocument([NotNull] T generic)
    {
        var document = generic.ConvertToDocument();
        _writer?.UpdateDocument(new Term(nameof(IDocument.UniqueKey), generic.UniqueKey), GetDocument(document));

        Commit();
    }

    public void RemoveDocument([NotNull] T generic)
    {
        _writer?.DeleteDocuments(new Term(nameof(IDocument.UniqueKey), generic.UniqueKey));

        Commit();
    }

    public void Dispose()
    {
        _taxonomyWriter?.Dispose();
        _facetIndexDirectory?.Dispose();
        _writer?.Dispose();

        _initialized = false;
    }

    private void Commit()
    {
        _writer?.Commit();
        _taxonomyWriter?.Commit();
    }

    private Document GetDocument(T generic)
    {
        var document = generic.ConvertToDocument();
        return GetDocument(document);
    }
    
    /// <summary>
    /// Gets the document with facets applied if configured.
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    private Document GetDocument(Document document)
    {
        return _facetsConfig != null ? _facetsConfig.Build(_taxonomyWriter, document) : document;
    }
}