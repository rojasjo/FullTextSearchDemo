using FullTextSearchDemo.SearchEngine.Models;

namespace FullTextSearchDemo.SearchEngine.Configuration;

/// <summary>
/// Represents the configuration for an index associated with documents of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of documents for which the index is configured, which must implement <see cref="IDocument"/>.</typeparam>
public interface IIndexConfiguration<T> where T : IDocument
{
    /// <summary>
    /// The name of the index associated with documents of type <typeparamref name="T"/>.
    /// This name is used to create a folder that stores the files required by Lucene for indexing.
    /// </summary>
    string IndexName { get; }
    
    /// <summary>
    /// The configuration for the facets associated with documents of type <typeparamref name="T"/>.
    /// If no facets are required, this property can be set to <see langword="null"/>.
    /// </summary>
    FacetConfiguration<T>? FacetConfiguration { get; }
}