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

internal sealed class DocumentWriter<T> : IDocumentWriter<T> where T : IDocument
{
    private readonly DirectoryTaxonomyWriter? _taxonomyWriter;

    public IndexWriter Writer { get; }

    public FacetsConfig? FacetsConfig { get; }

    public LuceneDirectory FacetIndexDirectory { get; }

    public DocumentWriter(IIndexConfiguration<T> configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.IndexName))
        {
            throw new ArgumentException("Index name must be set before using DocumentWriter.");
        }

        FacetsConfig = configuration.FacetConfiguration?.GetFacetConfig();

        // Open the index directories
        var indexPath = Path.Combine(Environment.CurrentDirectory, configuration.IndexName);
        var indexDirectory = FSDirectory.Open(indexPath);

        // Create an analyzer to process the text
        const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;
        Analyzer standardAnalyzer = new StandardAnalyzer(luceneVersion);
        var indexConfig = new IndexWriterConfig(luceneVersion, standardAnalyzer)
        {
            OpenMode = OpenMode.CREATE_OR_APPEND
        };

        // Create the index writer with the above configuration
        Writer = new IndexWriter(indexDirectory, indexConfig);

        if (FacetsConfig == null)
        {
            return;
        }

        FacetIndexDirectory = FSDirectory.Open($"{indexPath}-facets");
        _taxonomyWriter = new DirectoryTaxonomyWriter(FacetIndexDirectory);
    }

    public void AddDocument([NotNull] T generic)
    {
        var document = generic.ConvertToDocument();
        Writer.AddDocument(GetDocument(document));

        Writer.Commit();
        _taxonomyWriter?.Commit();
    }

    public void Clear()
    {
        Writer.DeleteAll();

        Writer.Commit();
        _taxonomyWriter?.Commit();
    }

    public void AddDocuments(IEnumerable<T> documents)
    {
        foreach (var generic in documents)
        {
            var document = generic.ConvertToDocument();
            Writer.AddDocument(GetDocument(document));
        }

        Writer.Commit();
        _taxonomyWriter?.Commit();
    }

    public void UpdateDocument([NotNull] T generic)
    {
        var document = generic.ConvertToDocument();
        Writer.UpdateDocument(new Term(nameof(IDocument.UniqueKey), generic.UniqueKey), GetDocument(document));

        Writer.Commit();
        _taxonomyWriter?.Commit();
    }

    public void RemoveDocument([NotNull] T generic)
    {
        Writer.DeleteDocuments(new Term(nameof(IDocument.UniqueKey), generic.UniqueKey));

        Writer.Commit();
        _taxonomyWriter?.Commit();
    }

    private Document GetDocument(Document document)
    {
        return FacetsConfig != null ? FacetsConfig.Build(_taxonomyWriter, document) : document;
    }
}