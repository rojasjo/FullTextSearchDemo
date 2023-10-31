using FullTextSearchDemo.SearchEngine.Models;

namespace FullTextSearchDemo.SearchEngine.Configuration;

public interface IIndexConfiguration<T>  where T : IDocument
{
    string IndexName { get; }
}