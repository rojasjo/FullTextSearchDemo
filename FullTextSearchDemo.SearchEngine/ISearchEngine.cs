using FullTextSearchDemo.SearchEngine.Models;
using FullTextSearchDemo.SearchEngine.Queries;
using FullTextSearchDemo.SearchEngine.Results;

namespace FullTextSearchDemo.SearchEngine;

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
    /// Adds a collection of documents to the search engine.
    /// </summary>
    void AddRange(IEnumerable<T> documents);
    
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
    SearchResult<T> Search(SearchQuery searchQuery);
    
    /// <summary>
    /// Removes all documents from the search engine.
    /// </summary>
    void RemoveAll();

    /// <summary>
    /// Disposes the in memory resources used by the search engine.
    /// The search engine can be used again after calling this method.
    /// </summary>
    void DisposeResources();
}