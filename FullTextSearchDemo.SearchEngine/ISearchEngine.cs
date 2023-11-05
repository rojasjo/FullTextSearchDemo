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
    SearchResult<T> Search(FieldSpecificSearchQuery searchQuery);

    /// <summary>
    /// Performs a search operation using search criteria on all fields and returns the search result.
    /// </summary>
    /// <param name="searchQuery">The search query specifying search terms for all fields.</param>
    /// <returns>The search result containing documents matching the search criteria.</returns>
    SearchResult<T> Search(AllFieldsSearchQuery searchQuery);
    
    /// <summary>
    /// Performs a search operation using full-text search preconfigured query and returns the search result.
    /// </summary>
    /// <param name="searchQuery"></param>
    /// <returns></returns>
    SearchResult<T> Search(FullTextSearchQuery searchQuery);
    
    /// <summary>
    /// Clears the search engine by removing all documents.
    /// </summary>
    void Clear();
}