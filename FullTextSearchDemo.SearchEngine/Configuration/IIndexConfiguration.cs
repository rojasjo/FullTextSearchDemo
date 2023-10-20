namespace FullTextSearchDemo.SearchEngine.Configuration;

public interface IIndexConfiguration<T>  where T : class
{
    string IndexName { get; }
}