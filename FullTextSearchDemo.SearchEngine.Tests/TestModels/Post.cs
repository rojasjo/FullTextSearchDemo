using FullTextSearchDemo.SearchEngine.Models;

namespace FullTextSearchDemo.SearchEngine.Tests.TestModels;

public class Post : IDocument
{
    public string UniqueKey => Id.ToString();
    
    public long Id { get; set; }

    public required string Title { get; set; }

    public required string Content { get; set; }
}