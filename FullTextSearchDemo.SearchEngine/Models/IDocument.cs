namespace FullTextSearchDemo.SearchEngine.Models;

/// <summary>
/// Represents an interface for documents handled by the search library.
/// </summary>
public interface IDocument
{
    /// <summary>
    /// Gets the unique identifier for the document. This identifier must be unique for each document.
    /// Warning: If multiple documents share the same UniqueKey, operations like update and delete will affect all of them.
    /// </summary>
    string UniqueKey { get; }
}