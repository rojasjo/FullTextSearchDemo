using FullTextSearchDemo.SearchEngine.Models;

namespace FullTextSearchDemo.SearchEngine.Engine;

/// <summary>
/// Defines the interface for a search engine that operates on documents of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of documents the search engine operates on, which must implement <see cref="IDocument"/>.</typeparam>
public interface ISearchEngine<T> where T : IDocument
{
    /// <summary>
    /// Adds a document to the search engine.
    /// </summary>
    void Add(T document);

    /// <summary>
    /// Updates an existing document in the search engine.
    /// </summary>
    void Update(T document);

    /// <summary>
    /// Removes a document from the search engine.
    /// </summary>
    void Remove(T document);
    
    /// <summary>
    /// Performs a search operation using field-specific search criteria and returns the search result.
    /// </summary>
    /// <param name="searchQuery">The search query specifying field-specific search terms.</param>
    /// <returns>The search result containing documents matching the search criteria.</returns>
    public SearchResult<T> Search(FieldSpecificSearchQuery searchQuery);
    
    /// <summary>
    /// Performs a search operation using search criteria on all fields and returns the search result.
    /// </summary>
    /// <param name="searchQuery">The search query specifying search terms for all fields.</param>
    /// <returns>The search result containing documents matching the search criteria.</returns>
    public SearchResult<T> Search(AllFieldsSearchQuery searchQuery);
}